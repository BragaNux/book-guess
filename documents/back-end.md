# BookGuess - Backend Architecture & Engineering Guidelines

# Objetivo

Construir um backend moderno, escalável e desacoplado para suportar a plataforma BookGuess.

O backend deve ser responsável por:

* Regras de negócio
* Autenticação
* Gamificação
* Rankings
* Progressão
* Integração com IA
* Pipeline RAG
* Persistência de dados

A arquitetura deve seguir princípios de Clean Architecture, SOLID e Domain-Driven Design simplificado.

---

# Stack Tecnológica

## API Principal

Tecnologia:

```text
ASP.NET Core 8
```

Responsabilidades:

* Regras de negócio
* Autenticação
* Pontuação
* Rankings
* Usuários
* Jogos
* Administração

---

## AI Service

Tecnologia:

```text
ASP.NET Core Minimal API
```

Responsabilidades:

* Pipeline RAG
* Embeddings
* Retrieval
* Integração Claude

---

## Banco de Dados

```text
MySQL 8
```

---

## Banco Vetorial

```text
Qdrant
```

---

## Containerização

```text
Docker
Docker Compose
```

---

# Arquitetura Geral

```text
Frontend (Blazor)
        │
        ▼
Backend API
        │
 ┌──────┴─────────┐
 │                │
 ▼                ▼
MySQL       AI Service
                 │
                 ▼
           RAG Pipeline
                 │
                 ▼
              Qdrant
                 │
                 ▼
         Claude 3.5 Haiku
```

---

# Estrutura da Solução

```text
src/

BookGuess.Api

BookGuess.Application

BookGuess.Domain

BookGuess.Infrastructure

BookGuess.AIService

tests/

BookGuess.Tests
```

---

# Camadas

## Domain

Responsável por:

* Entidades
* Enums
* Value Objects
* Regras puras

Nunca deve conhecer:

* Banco
* HTTP
* Claude
* Frameworks

---

## Application

Responsável por:

* Casos de uso
* Serviços
* DTOs
* Validações

Exemplos:

```text
StartMatch

SubmitGuess

GenerateHint

GetRanking

AwardAchievement
```

---

## Infrastructure

Responsável por:

* Entity Framework
* Repositórios
* Integrações externas
* Qdrant
* Claude

---

## API

Responsável por:

* Controllers
* Middlewares
* Auth
* Swagger

Nunca conter regra de negócio.

---

# Entidades

## User

```text
Id
Name
Email
PasswordHash

Level
XP

CurrentStreak
BestStreak

CreatedAt
```

---

## Book

```text
Id

Title
Author

Genre

Description

Difficulty

CreatedAt
```

---

## BookQuote

```text
Id

BookId

Content

Difficulty

CreatedAt
```

---

## Match

```text
Id

UserId

QuoteId

Score

Attempts

Status

CreatedAt
```

---

## Guess

```text
Id

MatchId

GuessText

IsCorrect

CreatedAt
```

---

## Achievement

```text
Id

Name

Description

XPReward
```

---

## UserAchievement

```text
UserId

AchievementId

UnlockedAt
```

---

# Gamificação

## Sistema de XP

Exemplo:

```text
Acerto:
+100 XP

Primeira tentativa:
+50 XP

Sem dicas:
+25 XP

Streak:
+10 XP
```

---

## Sistema de Níveis

```text
Nível 1 → 0 XP

Nível 2 → 500 XP

Nível 3 → 1000 XP

Nível 4 → 2000 XP
```

Progressão crescente.

---

## Sistema de Streak

Atualizado diariamente.

Objetivos:

* Retenção
* Engajamento

---

# Casos de Uso

## StartMatch

Responsável por:

* Selecionar trecho
* Criar partida
* Retornar estado inicial

---

## SubmitGuess

Responsável por:

* Validar resposta
* Registrar tentativa
* Atualizar pontuação

---

## RequestHint

Responsável por:

* Aplicar penalidade
* Solicitar dica à IA
* Registrar uso

---

## FinishMatch

Responsável por:

* Finalizar partida
* Calcular XP
* Atualizar ranking

---

## GetDailyChallenge

Responsável por:

* Retornar desafio diário

Mesmo desafio para todos os usuários.

---

# API REST

## Auth

```http
POST /api/auth/login
POST /api/auth/register
```

---

## Matches

```http
POST /api/matches/start

POST /api/matches/{id}/guess

POST /api/matches/{id}/hint

POST /api/matches/{id}/finish
```

---

## Ranking

```http
GET /api/ranking/global

GET /api/ranking/weekly

GET /api/ranking/monthly
```

---

## Profile

```http
GET /api/profile

GET /api/profile/stats

GET /api/profile/achievements
```

---

## Books

```http
GET /api/books

GET /api/books/{id}
```

---

# AI Service

Responsável exclusivamente por IA.

Nenhum componente do sistema acessa Claude diretamente.

---

# Pipeline RAG

Fluxo:

```text
Livro
 ↓
Ingestão
 ↓
Chunking
 ↓
Embeddings
 ↓
Qdrant
 ↓
Retrieval
 ↓
Claude
 ↓
Resposta
```

---

# Ingestion Service

Responsável por:

* Importar livros
* Processar metadados
* Preparar documentos

Formatos:

```text
JSON
TXT
Markdown
```

Evitar PDFs na primeira versão.

---

# Chunking

Estratégia:

```text
Chunk Size:
400 tokens

Overlap:
80 tokens
```

Objetivo:

Melhor recuperação semântica.

---

# Embeddings

Responsável por converter:

```text
Texto
```

em

```text
Vetores
```

Os embeddings serão armazenados no Qdrant.

---

# Qdrant

Responsável por:

* Armazenamento vetorial
* Busca semântica
* Similaridade

Toda recuperação de contexto deve passar pelo Qdrant.

---

# Claude Provider

Modelo:

```text
Claude 3.5 Haiku
```

Responsabilidades:

* Dicas
* Curiosidades
* Explicações
* Resumos

Nunca responder sem contexto recuperado.

---

# Prompts

Todos os prompts devem ser centralizados.

Estrutura:

```text
Prompts/

GenerateHint.txt

ExplainMistake.txt

GenerateTrivia.txt
```

Proibido hardcode de prompts espalhados pelo código.

---

# Segurança

## JWT

Autenticação baseada em:

```text
JWT Bearer
```

---

## Senhas

Utilizar:

```text
BCrypt
```

---

## Secrets

Nunca armazenar:

* Claude Key
* JWT Secret
* Connection Strings

no código-fonte.

Utilizar:

```env
CLAUDE_API_KEY=

JWT_SECRET=

MYSQL_CONNECTION=

QDRANT_URL=
```

---

# Observabilidade

Implementar:

* Logs estruturados
* Correlation Id
* Tratamento global de exceções

---

# Docker

Cada componente deve possuir seu próprio Dockerfile.

Containers:

```text
frontend

backend

ai-service

mysql

qdrant

nginx
```

---

# Docker Compose

Todo ambiente deve subir com:

```bash
docker compose up -d
```

---

# Deploy

Servidor:

```text
Ubuntu 24.04
```

Infraestrutura:

```text
Docker
Docker Compose
Nginx
```

Deploy:

```bash
git pull

docker compose build

docker compose up -d
```

---

# Testes

Implementar:

## Unitários

* Regras de XP
* Regras de Ranking
* Regras de Progressão

---

## Integração

* API
* Banco
* AI Service

---

# Regras de Engenharia

Nunca:

* Misturar regra de negócio com Controllers
* Acessar banco diretamente da API
* Chamar Claude diretamente do Frontend
* Duplicar lógica de pontuação

Sempre:

* Utilizar injeção de dependência
* Centralizar regras
* Escrever código testável
* Manter baixo acoplamento

---

# Objetivo Final

O backend deve parecer uma aplicação de produção real.

A arquitetura deve demonstrar:

* Desenvolvimento Full Stack
* Engenharia de Software
* Containers
* APIs REST
* Banco Relacional
* Banco Vetorial
* RAG
* Integração com LLM
* Boas práticas de mercado

O projeto deve ser suficientemente robusto para ser apresentado como um produto SaaS real e não apenas como um trabalho acadêmico.
