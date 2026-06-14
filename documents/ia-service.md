# BookGuess - AI Service, RAG & LLM Architecture

# Objetivo

Construir uma camada de Inteligência Artificial desacoplada da aplicação principal, utilizando arquitetura RAG (Retrieval-Augmented Generation) para fornecer respostas contextualizadas e reduzir alucinações.

O sistema deve ser capaz de:

* Gerar dicas inteligentes
* Explicar respostas incorretas
* Gerar curiosidades literárias
* Produzir resumos de obras
* Recuperar contexto relevante do acervo
* Operar com baixo custo utilizando Claude 3.5 Haiku

---

# Filosofia da Solução

O modelo de IA não deve ser utilizado como fonte única de conhecimento.

Toda resposta deve ser fundamentada em informações recuperadas da base literária do sistema.

Princípio:

```text
Retrieve First
Generate Second
```

O modelo apenas gera a resposta final.

O conhecimento vem da base vetorial.

---

# Arquitetura Geral

```text
Backend API
      │
      ▼
AI Service
      │
      ▼
RAG Pipeline
      │
 ┌────┼─────────────────────────┐
 │    │                         │
 ▼    ▼                         ▼
Chunking Embeddings       Retrieval
 │    │                         │
 └────┴────────► Qdrant ◄────────┘
                     │
                     ▼
             Claude 3.5 Haiku
                     │
                     ▼
                 Resposta
```

---

# Responsabilidades

O AI Service é responsável por:

* Processamento documental
* Geração de embeddings
* Busca semântica
* Construção de prompts
* Integração com Claude
* Orquestração RAG

O AI Service não é responsável por:

* Usuários
* Ranking
* Partidas
* XP
* Regras de negócio

Essas responsabilidades pertencem ao Backend API.

---

# Stack Tecnológica

## Linguagem

```text
C#
.NET 8
```

---

## Framework

```text
ASP.NET Core Minimal API
```

---

## Banco Vetorial

```text
Qdrant
```

---

## Modelo Principal

```text
Claude 3.5 Haiku
```

---

## Containerização

```text
Docker
Docker Compose
```

---

# Estrutura do Projeto

```text
BookGuess.AIService

├── Controllers

├── Application

│   ├── Services
│   ├── Pipelines
│   ├── Prompts

├── Domain

│   ├── Models
│   ├── Contracts

├── Infrastructure

│   ├── Claude
│   ├── Embeddings
│   ├── Qdrant
│   ├── Storage

├── Documents

└── Docker
```

---

# Pipeline RAG

## Etapa 1

Ingestão

Entrada:

```text
Livro
Trecho
Metadados
Descrição
```

---

## Etapa 2

Chunking

Responsável por dividir conteúdos em blocos semanticamente relevantes.

Exemplo:

```text
Livro Completo

↓

Chunk 01
Chunk 02
Chunk 03
Chunk 04
```

---

## Etapa 3

Embeddings

Cada chunk é convertido em vetor semântico.

Entrada:

```text
Capitu tinha olhos de ressaca.
```

Saída:

```text
[0.123, 0.821, 0.554, ...]
```

---

## Etapa 4

Persistência Vetorial

Os embeddings são armazenados no Qdrant.

Metadados associados:

```json
{
  "bookId": 1,
  "title": "Dom Casmurro",
  "author": "Machado de Assis",
  "chunkId": 42
}
```

---

## Etapa 5

Retrieval

Quando uma solicitação é realizada:

```text
Gerar dica
```

O sistema busca:

```text
Chunks mais relevantes
```

---

## Etapa 6

Prompt Building

O contexto recuperado é inserido no prompt.

Estrutura:

```text
Contexto Recuperado

+
Instruções do Sistema

+
Pergunta do Usuário
```

---

## Etapa 7

Geração

Claude produz a resposta final.

---

# Fluxo de Dica

```text
Usuário
     │
     ▼
Pedir Dica
     │
     ▼
Backend API
     │
     ▼
AI Service
     │
     ▼
Retrieval
     │
     ▼
Qdrant
     │
     ▼
Contexto
     │
     ▼
Claude
     │
     ▼
Dica
```

---

# Fluxo de Curiosidade

```text
Livro Descoberto
      │
      ▼
Retrieval
      │
      ▼
Contexto
      │
      ▼
Claude
      │
      ▼
Curiosidade
```

---

# Fluxo de Explicação

Entrada:

```json
{
  "guess": "Dom Quixote",
  "correctBook": "Dom Casmurro"
}
```

Fluxo:

```text
Busca Contexto
      │
      ▼
Prompt
      │
      ▼
Claude
      │
      ▼
Explicação
```

---

# Chunking Strategy

## Tamanho

```text
400 Tokens
```

---

## Overlap

```text
80 Tokens
```

---

## Objetivo

* Preservar contexto
* Melhorar recuperação
* Evitar perda semântica

---

# Qdrant Collections

## Books

Armazena:

```text
Descrições
Resumos
Informações da obra
```

---

## Quotes

Armazena:

```text
Trechos
Passagens
Fragmentos
```

---

## Trivia

Armazena:

```text
Curiosidades
Contexto histórico
Informações adicionais
```

---

# Claude Provider

## Modelo

```text
Claude 3.5 Haiku
```

---

## Motivos

* Baixo custo
* Alta velocidade
* Excelente compreensão textual
* Ótimo para RAG
* Baixa latência

---

# Prompt Engineering

Todos os prompts devem ser centralizados.

Estrutura:

```text
Prompts/

GenerateHint.txt

GenerateTrivia.txt

ExplainMistake.txt

SummarizeBook.txt
```

---

# Regras de Prompt

Nunca:

* Revelar título diretamente
* Revelar autor diretamente
* Entregar resposta ao usuário

Sempre:

* Produzir dicas graduais
* Respeitar nível de dificuldade
* Utilizar contexto recuperado

---

# Sistema de Dicas

## Nível 1

Muito vaga.

Exemplo:

```text
Esta obra é considerada um clássico da literatura brasileira.
```

---

## Nível 2

Moderada.

```text
A narrativa apresenta um protagonista marcado por dúvidas constantes.
```

---

## Nível 3

Forte.

```text
A obra discute uma relação famosa entre Bentinho e Capitu.
```

---

# Cache

Implementar cache para:

* Curiosidades
* Dicas
* Resumos

Objetivo:

* Reduzir custos
* Reduzir chamadas ao Claude

---

# Rate Limiting

Implementar proteção.

Exemplo:

```text
20 requisições por minuto
```

por usuário.

---

# Observabilidade

Implementar:

## Logs

Registrar:

```text
Prompt
Tempo de execução
Tokens
Modelo utilizado
```

Sem armazenar informações sensíveis.

---

## Métricas

Monitorar:

```text
Tempo médio
Uso de tokens
Erros
Latência
```

---

# Segurança

Nunca expor:

```text
CLAUDE_API_KEY
```

ao Frontend.

O Frontend nunca deve acessar Claude diretamente.

---

# Variáveis de Ambiente

```env
CLAUDE_API_KEY=

QDRANT_URL=

QDRANT_API_KEY=

EMBEDDING_PROVIDER=
```

---

# Endpoints

## Gerar Dica

```http
POST /api/ai/hint
```

---

## Gerar Curiosidade

```http
POST /api/ai/trivia
```

---

## Explicar Erro

```http
POST /api/ai/explain
```

---

## Resumir Livro

```http
POST /api/ai/summary
```

---

# Docker

Container:

```text
bookguess-ai
```

Dependências:

```text
qdrant
```

---

# Objetivos de Qualidade

Latência:

```text
< 3 segundos
```

---

Precisão:

```text
Alta
```

---

Alucinação:

```text
Mínima
```

---

Escalabilidade:

```text
Alta
```

---

# Visão de Longo Prazo

A arquitetura deve permitir futuramente:

* Troca de Claude por OpenAI
* Troca de Claude por Gemini
* Múltiplos modelos simultâneos
* Agentes especializados
* Sistema de recomendação de livros
* Geração automática de novos desafios

Sem necessidade de reescrever a aplicação principal.

---

# Regra Fundamental

O Claude nunca é a fonte da verdade.

A fonte da verdade é o conhecimento recuperado do RAG.

O modelo apenas transforma contexto em respostas úteis para o usuário.
