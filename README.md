# 🛡️ Fallback LLM Resilience API

[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Supported-blue.svg)](https://www.docker.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A robust, production-ready .NET 9 Minimal API demonstrating how to achieve **high availability (100% uptime)** for LLM-powered features. This project uses the **Decorator Design Pattern** and Microsoft's unified `Microsoft.Extensions.AI` abstraction to seamlessly failover from a primary cloud AI provider (OpenAI) to a local, self-hosted AI model (Ollama) when disruptions occur.

## 💡 The Core Insight: Why Fallback Strategy?
In modern applications, LLM agents generate critical content—like dynamic product descriptions for e-commerce. Relying on a single cloud provider creates a single point of failure. API rate limits, network timeouts, or service outages can break your application.

This architecture solves that by implementing a **Fallback Strategy**:
1. **Primary Attempt:** The system attempts to stream the AI response using the primary provider (e.g., OpenAI `gpt-4o`).
2. **Graceful Failover:** If the primary provider is unreachable or the stream breaks mid-generation, the `FallbackChatClient` (a Decorator) intercepts the exception.
3. **Local Rescue:** It instantly switches the workload to a local Ollama instance (e.g., `qwen2.5:3b`), ensuring the end-user always receives a response without noticing the backend crisis.

## ✨ Key Features
* **Seamless AI Abstraction:** Utilizes `Microsoft.Extensions.AI.IChatClient` to standardize interactions across different LLMs.
* **Streaming Responses (`IAsyncEnumerable`):** Delivers AI-generated content word-by-word for an interactive user experience.
* **Robust Validation:** Uses `FluentValidation` for comprehensive payload validation, ensuring only high-quality data reaches the LLM to minimize token waste.
* **Zero-Touch Local AI Setup:** The included `docker-compose.yaml` not only spins up the API and Ollama but uses a custom bootstrapper to automatically pull the required local model (`qwen2.5:3b`) on the first run.

## 🚀 Getting Started

### Prerequisites
* Docker and Docker Compose installed on your machine.
* An OpenAI API Key.

### Installation & Run
1. Clone the repository.
2. Open `compose.yaml` in the root directory and replace `YOUR_OPENAI_API_KEY` with your actual API key.
3. Run the following command in your terminal:

```bash
docker-compose up -build -d
```

> **Note:** On the first run, the `ollama-ready` service will take a few minutes to download the `qwen2.5:3b` model. The API will wait for this process to complete before starting.

## 🔌 API Usage

**Endpoint:** `POST /api/products/generate-description`

Generate an engaging product description based on technical features.

**Request Body (JSON):**
```json
{
  "productName": "Wireless Gaming Headset X-100",
  "features": [
    { "name": "Connectivity", "value": "Bluetooth 5.3" },
    { "name": "Battery Life", "value": "40 Hours" },
    { "name": "Noise Cancellation", "value": "Active (ANC)" }
  ],
  "tone": "Exciting"
}
```
### Testing the Stream
To see the true streaming effect, use curl in your terminal instead of Postman (which buffers the response):
```bash
curl -N -X POST http://localhost:5125/api/products/generate-description \
     -H "Content-Type: application/json" \
     -d '{"productName": "Gaming Headset", "features": [{"name": "Sound", "value": "7.1 Surround"}], "tone": "Professional"}'
```

## 🏗️ Project Structure
* **Abstractions/ & Services/:** Core business logic and the LLM agent implementation.
* **Infrastructure/:** Contains the heart of the project, the FallbackChatClient decorator.
* **DTOs/ & Validators/:** Data contracts and FluentValidation rules. 
* **Extensions/:** Clean Dependency Injection configuration.