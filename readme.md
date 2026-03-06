# 📦 Intelligent Logistics Gateway

Um sistema de mensageria escalável para logística, focado em **alta disponibilidade** e **desacoplamento de serviços**.

## 🛠️ Stack Técnica
- **Linguagem:** C# (.NET 8)
- **Mensageria:** Apache Kafka
- **Persistência:** Entity Framework Core (SQLite)
- **Ambiente:** Docker & Docker Compose

## 🏗️ Arquitetura
O sistema utiliza uma **Arquitetura Orientada a Eventos (EDA)**:
1. **ILG.Api**: Recebe pedidos de carga via REST e produz eventos para o Kafka.
2. **Kafka Broker**: Garante a retenção e durabilidade das mensagens.
3. **ILG.Worker**: Consome os eventos de forma assíncrona e persiste no banco de dados, garantindo que a API não seja sobrecarregada.

## 🚀 Como Executar
1. Suba a infraestrutura: `docker-compose up -d`
2. Execute o Worker: `dotnet run --project ILG.Worker`
3. Execute a API: `dotnet run --project ILG.Api`
4. Acesse o Swagger em `http://localhost:5000/swagger`

## 🏗️ Arquitetura do Sistema

O fluxo de dados segue o padrão de Mensageria Assíncrona:

### 🏗️ Arquitetura do Sistema

```mermaid
graph TD
    A[📱 Swagger/Client] -->|HTTP POST| B(⚙️ .NET API - Producer)
    B -->|Produce Message| C{📨 Kafka Broker}
    C -->|Consume Event| D(🛠️ Worker Service - Consumer)
    D -->|Persist Data| E[(🗄️ SQLite Database)]
    
    style C fill:#f96,stroke:#333,stroke-width:2px
    style E fill:#00758f,color:#fff
    


Client (Swagger): Dispara uma requisição HTTP POST com o JSON da carga.
ILG.Api (Producer): Valida a entrada e faz o Produce (envio) para o Kafka. Ela retorna 202 Accepted imediatamente, liberando o cliente.
Kafka (Message Broker): Atua como o buffer de resiliência. Ele armazena a mensagem em disco no tópico order-received.
ILG.Worker (Consumer): Escuta o tópico de forma reativa. Quando há uma mensagem, ele a "puxa" (Consume), processa e confirma a leitura (commit).


📄 Como descrever isso no seu Currículo/LinkedIn?
Use estes termos técnicos para valorizar o que você fez:

"Desenvolvimento de Arquitetura Orientada a Eventos (EDA) utilizando Apache Kafka para processamento assíncrono."
"Implementação de Background Services em .NET 8 para consumo resiliente de mensagens."
"Orquestração de infraestrutura de mensageria utilizando Docker e Docker Compose."

