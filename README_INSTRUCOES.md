# Comparação de Algoritmos de Busca de Strings

Este projeto é uma ferramenta de análise de performance e observabilidade para diferentes algoritmos de busca de strings (Naive, Rabin-Karp, KMP e Boyer-Moore). Ele consiste em uma **Web API (.NET 9)** instrumentada com OpenTelemetry e um **Frontend (React)** para visualização de dados e gráficos.

---

## Como Rodar o Projeto

### Pré-requisitos
*   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
*   [Node.js (v18 ou superior)](https://nodejs.org/)
*   [Docker](https://www.docker.com/) (Opcional, apenas se quiser visualizar dados no Jaeger/Prometheus)

---

### 1. Backend (Web API)

O backend processa os algoritmos e gera métricas de performance.

1.  Abra um terminal na raiz do projeto.
2.  Navegue até a pasta do backend:
    ```bash
    cd string-search-comparison
    ```
3.  Execute a aplicação:
    ```bash
    dotnet run
    ```
4.  A API estará rodando em: `http://localhost:5000`
5.  Você pode acessar a documentação interativa (Swagger) em: `http://localhost:5000/swagger`

---

### 2. Frontend (React + Vite)

O frontend permite carregar arquivos, configurar buscas e visualizar os gráficos de performance.

1.  Abra um **novo terminal** na raiz do projeto.
2.  Navegue até a pasta do frontend:
    ```bash
    cd frontend
    ```
3.  Instale as dependências (necessário apenas na primeira vez):
    ```bash
    npm install
    ```
4.  Inicie o servidor de desenvolvimento:
    ```bash
    npm run dev
    ```
5.  O sistema estará disponível em: `http://localhost:5173` (ou a porta indicada no terminal).

---

## Funcionalidades

1.  **Carregamento de Texto:**
    *   Upload de arquivos `.txt`.
    *   Entrada de texto manual (suporta Unicode e Emojis).
2.  **Busca e Comparação:**
    *   Execução de um algoritmo específico.
    *   Botão **"Comparar Todos"**: Executa todos os algoritmos simultaneamente e gera gráficos comparativos.
3.  **Dashboard de Performance:**
    *   Gráfico de **Tempo de Execução (μs)**.
    *   Gráfico de **Comparações de Caracteres**.
    *   Tabela detalhada com complexidade teórica e tempo em nanossegundos.
4.  **Observabilidade:**
    *   Instrumentado com **OpenTelemetry**.
    *   Logs estruturados e métricas de performance automáticas.

---

## Tecnologias Utilizadas

*   **Backend:** C# (.NET 9), ASP.NET Core Minimal APIs, OpenTelemetry.
*   **Frontend:** React, TypeScript, Vite, Tailwind CSS, Recharts (Gráficos), Lucide React (Ícones).
