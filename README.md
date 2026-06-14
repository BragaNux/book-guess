# BookGuess

> Jogo de adivinhação literária com desafios diários, dicas geradas por IA e sistema de ranking.

---

## Sumário

1. [Visão Geral](#1-visão-geral)
2. [Arquitetura da Solução](#2-arquitetura-da-solução)
3. [Tecnologias Utilizadas](#3-tecnologias-utilizadas)
4. [Estrutura de Diretórios](#4-estrutura-de-diretórios)
5. [Módulos e Arquivos](#5-módulos-e-arquivos)
   - [Domain](#51-bookguessdomainentities)
   - [Application](#52-bookguessapplication)
   - [Infrastructure](#53-bookguessinfrastructure)
   - [API](#54-bookguessapi)
   - [AI Service](#55-bookguessaiservice)
   - [Frontend Web](#56-bookguessweb)
6. [Fluxo de Execução](#6-fluxo-de-execução)
7. [Frontend em Detalhe](#7-frontend-em-detalhe)
8. [Backend em Detalhe](#8-backend-em-detalhe)
9. [Banco de Dados](#9-banco-de-dados)
10. [APIs](#10-apis)
11. [Segurança](#11-segurança)
12. [Configuração e Infraestrutura](#12-configuração-e-infraestrutura)
13. [Dependências do Projeto](#13-dependências-do-projeto)
14. [Regras de Negócio](#14-regras-de-negócio)
15. [Resumo Executivo](#15-resumo-executivo)

---

## 1. Visão Geral

### O que é o BookGuess?

BookGuess é um jogo de adivinhação literária baseado na web. O jogador recebe um **trecho de um livro clássico da literatura brasileira** e deve identificar a obra ou o autor dentro de até **5 tentativas**. A cada erro a pontuação cai; o jogador pode pedir até **3 dicas** geradas por IA para orientar o palpite.

### Problema resolvido

Estimular o contato com a literatura brasileira de forma lúdica, criando um hábito diário de leitura/reconhecimento similar ao Wordle, mas focado em obras literárias nacionais.

### Usuários-alvo

Leitores brasileiros de qualquer nível, estudantes de literatura e entusiastas de jogos de palavras/cultura.

### Fluxo geral

```
Usuário abre o site
  → (não logado) vê landing page → registra/loga
  → (logado) escolhe Desafio Diário ou Partida Livre
  → lê o trecho exibido
  → envia palpite (título ou autor)
  → acerto → tela de resultado com trivia da IA + XP ganho
  → erro   → penalidade de pontos; pode pedir dica da IA
  → 5 erros → partida encerrada, revela o livro + trivia
```

### Principais módulos

| Módulo | Responsabilidade |
|---|---|
| `BookGuess.Domain` | Entidades, enums, interfaces de repositório |
| `BookGuess.Application` | Use Cases, DTOs, interfaces de serviços externos |
| `BookGuess.Infrastructure` | EF Core, repositórios, JWT, DataSeeder, cliente HTTP para IA |
| `BookGuess.Api` | Controllers REST, middlewares, pipeline HTTP, Swagger |
| `BookGuess.AIService` | Microserviço de IA: dicas, trivia, RAG com Qdrant |
| `BookGuess.Web` | Frontend Blazor Server: páginas, componentes, estado |

---

## 2. Arquitetura da Solução

### Padrão: Clean Architecture + Microserviço de IA

O backend principal segue **Clean Architecture** em camadas concêntricas onde a dependência sempre aponta para dentro:

```
┌─────────────────────────────────────────────┐
│              BookGuess.Api                  │  ← Entrypoint HTTP, Controllers
├─────────────────────────────────────────────┤
│          BookGuess.Infrastructure           │  ← EF Core, Repositórios, JWT
├─────────────────────────────────────────────┤
│          BookGuess.Application              │  ← Use Cases, DTOs, Interfaces
├─────────────────────────────────────────────┤
│            BookGuess.Domain                 │  ← Entidades, Regras de negócio
└─────────────────────────────────────────────┘
```

O **AI Service** é um microserviço independente em ASP.NET Core Minimal API que expõe endpoints HTTP consumidos pela camada Infrastructure do backend principal via `HttpClient` nomeado.

O **Frontend** (Blazor Server) é uma aplicação separada que se comunica com o backend via HTTP/REST usando `HttpClient`.

O **Nginx** é o ponto de entrada único: roteia `/api/*` para o backend e todo o resto para o frontend, incluindo o WebSocket do Blazor SignalR (`/_blazor`).

### Diagrama de Serviços

```
Browser
  │  HTTP/WebSocket
  ▼
┌─────────┐
│  Nginx  │ :80
└────┬────┘
     │ /api/*       → backend:8080
     │ /_blazor     → frontend:8080 (WebSocket SignalR)
     │ /*           → frontend:8080

┌──────────┐     HTTP      ┌────────────┐
│ Backend  │──────────────▶│ AI Service │
│ :8080    │               │ :8080      │
└────┬─────┘               └─────┬──────┘
     │ EF Core                   │ Claude API (HTTPS)
     ▼                           │ Qdrant (gRPC)
┌────────┐               ┌───────┴──────┐
│ MySQL  │               │    Qdrant    │
│  :3306 │               │  :6333/6334  │
└────────┘               └──────────────┘
```

### Fluxo de uma requisição autenticada

```
1. Browser → Nginx → Backend (Header: Authorization: Bearer <jwt>)
2. GlobalExceptionMiddleware (wrap try/catch)
3. UseAuthentication() → valida JWT → popula ClaimsPrincipal
4. UseAuthorization() → verifica [Authorize]
5. Controller → Use Case (Application)
6. Use Case → Repository (Infrastructure) → DbContext → MySQL
7. Use Case → AIServiceClient → AI Service → Claude API
8. Resposta: ApiResponse<T> { Success, Data, Message }
```

---

## 3. Tecnologias Utilizadas

### Runtime e Framework

| Tecnologia | Versão | Função | Por que foi escolhida |
|---|---|---|---|
| **.NET 9** | 9.x | Runtime base de todos os projetos | LTS mais recente com performance máxima e suporte a C# 13 |
| **ASP.NET Core 9** | 9.x | Framework HTTP para API e frontend | Integrado ao .NET, alta performance, suporte a Minimal API e Controllers |
| **Blazor Server** | 9.x | Framework de UI do frontend | Permite escrever frontend em C#, sem necessidade de JS; estado mantido no servidor via SignalR |

### Banco de Dados

| Tecnologia | Versão | Função |
|---|---|---|
| **MySQL 8.0** | 8.0 | Banco de dados relacional principal |
| **EF Core 9** | 9.x | ORM; mapeamento objeto-relacional, migrations |
| **Pomelo.EntityFrameworkCore.MySql** | 9.x | Provider MySQL para EF Core; detecta automaticamente versão do servidor |

### Inteligência Artificial

| Tecnologia | Função |
|---|---|
| **Anthropic.SDK** | SDK oficial da Anthropic para chamadas à API Claude |
| **claude-haiku-4-5-20251001** | Modelo Claude usado para gerar dicas e trivia; escolhido por velocidade e custo |
| **Qdrant** | Banco de vetores para o pipeline RAG; armazena embeddings dos trechos |
| **Ollama (all-minilm)** | Serviço local de embeddings para RAG; não incorre em custo de API |

### Autenticação

| Tecnologia | Função |
|---|---|
| **JWT Bearer** | Tokens stateless para autenticação; `Microsoft.AspNetCore.Authentication.JwtBearer` |
| **BCrypt.Net** | Hash seguro de senhas com salt automático |

### Frontend

| Tecnologia | Função |
|---|---|
| **Blazored.LocalStorage** | Persiste o JWT no `localStorage` do browser para sobreviver a recargas de página |
| **Lucide Icons (SVG inline)** | Ícones do sistema renderizados como SVG puro via componente `Icon.razor`, sem CDN externo |

### Infraestrutura

| Tecnologia | Função |
|---|---|
| **Docker / Docker Compose** | Orquestra 6 containers com rede interna isolada |
| **Nginx (alpine)** | Reverse proxy: roteamento de rotas, headers, WebSocket upgrade para SignalR |

---

## 4. Estrutura de Diretórios

```text
book-guess/
├── .env                          # Variáveis de ambiente (NÃO commitado)
├── .env.example                  # Template de variáveis de ambiente
├── .gitignore
├── BookGuess.sln                 # Solution file do .NET
├── docker-compose.yml            # Orquestração dos 6 containers
├── nginx/
│   └── nginx.conf                # Configuração do reverse proxy
├── documents/                    # Documentação e planejamento do projeto
│   ├── prd.md
│   ├── back-end.md
│   ├── front-end.md
│   ├── ia-service.md
│   ├── domain-model.md
│   ├── agent-rules.md
│   └── tasks.md
├── src/
│   ├── BookGuess.Domain/         # Camada de domínio (entidades, regras, interfaces)
│   ├── BookGuess.Application/    # Camada de aplicação (Use Cases, DTOs)
│   ├── BookGuess.Infrastructure/ # Camada de infraestrutura (EF Core, repos, JWT)
│   ├── BookGuess.Api/            # API REST (controllers, middlewares, program)
│   ├── BookGuess.AIService/      # Microserviço de IA (Claude + RAG)
│   └── BookGuess.Web/            # Frontend Blazor Server
└── tests/
    └── BookGuess.Tests/          # Testes de domínio
```

### Responsabilidade de cada pasta em `src/`

| Pasta | Responsabilidade | Depende de |
|---|---|---|
| `BookGuess.Domain` | Regras de negócio puras, entidades com comportamento, enums, interfaces de repositório | Ninguém |
| `BookGuess.Application` | Orquestra Use Cases; declara contratos (interfaces) com o mundo externo | Domain |
| `BookGuess.Infrastructure` | Implementa contratos: DbContext, repos, JWT, cliente HTTP de IA | Domain + Application |
| `BookGuess.Api` | Entry point HTTP; recebe requests, chama Use Cases, devolve respostas | Application + Infrastructure |
| `BookGuess.AIService` | Microserviço autônomo de IA; expõe endpoints HTTP consumidos pelo backend | Nenhuma dependência interna |
| `BookGuess.Web` | Frontend Blazor; chama API via HttpClient, mantém estado com `AppState` | Nenhuma dependência interna |

---

## 5. Módulos e Arquivos

### 5.1 `BookGuess.Domain/Entities`

#### `User.cs`

Entidade central do sistema. Encapsula toda a lógica de progressão do usuário.

```csharp
public class User
{
    public Guid Id { get; private set; }
    public string Name, Email, PasswordHash;
    public int Level { get; private set; } = 1;   // 1–5
    public int XP { get; private set; } = 0;
    public int CurrentStreak, BestStreak;
    public bool IsActive;
    // ...
}
```

| Método | O que faz |
|---|---|
| `Create(name, email, hash)` | Factory method; cria usuário com valores padrão |
| `AddXP(amount)` | Adiciona XP e recalcula o nível automaticamente |
| `IncrementStreak()` | Incrementa sequência; atualiza `BestStreak` se necessário |
| `ResetStreak()` | Zera a sequência atual |
| `UpdateLevel()` | Privado; tabela de XP: 500→Nv2, 1000→Nv3, 2000→Nv4, 3500→Nv5 |

#### `Match.cs`

Representa uma partida (ativa, ganha, perdida ou abandonada). Contém as constantes de mecânica do jogo.

```csharp
public class Match
{
    public const int MaxAttempts = 5;
    public const int MaxHints    = 3;
    public const int BaseScore   = 1000;
    public const int ScorePenaltyPerError = 100;
    public const int ScorePenaltyPerHint  = 150;
}
```

| Método | O que faz |
|---|---|
| `Create(userId, bookQuoteId, isDaily)` | Cria partida com score 1000 e status Active |
| `RegisterIncorrectGuess()` | Decrementa score em 100; se atingiu MaxAttempts, chama `Lose()` |
| `RegisterCorrectGuess()` | Incrementa tentativas, muda status para Won |
| `UseHint()` | Decrementa score em 150; retorna false se limite atingido |
| `CalculateXP()` | Base 100 XP + bônus: 1ª tentativa (+50), sem dicas (+25), desafio diário (+100) |
| `Abandon()` | Muda status para Abandoned |

#### `Book.cs`

Representa um livro. Campo `Difficulty` vai de 1 (fácil) a 5 (difícil).

#### `BookQuote.cs`

Trecho extraído de um livro. Um livro pode ter múltiplos trechos. A dificuldade herda do livro.

#### `DailyChallenge.cs`

Associa um `BookQuote` a uma data específica. Garante que todos os jogadores tenham o mesmo desafio em um dado dia.

#### `Guess.cs`

Registro de uma tentativa: texto do palpite, tipo (`Book` ou `Author`), resultado.

#### `Achievement.cs`

Conquista desbloqueável. Possui `Code` único, nome, descrição, ícone emoji e recompensa em XP.

#### `UserAchievement.cs`

Tabela de junção `N:N` entre `User` e `Achievement`, com a data de desbloqueio.

#### `Trivia.cs`

Curiosidade sobre um livro. Populada pelo `DataSeeder` com fatos históricos e literários; também gerada dinamicamente pela IA ao final de cada partida.

---

### 5.2 `BookGuess.Application`

#### `DTOs/ApiResponse.cs`

Envelope padrão de todas as respostas da API:

```csharp
public class ApiResponse<T>
{
    public bool    Success { get; init; }
    public T?      Data    { get; init; }
    public string? Message { get; init; }

    public static ApiResponse<T> Ok(T data)          => new() { Success = true, Data = data };
    public static ApiResponse<T> Fail(string message) => new() { Success = false, Message = message };
}
```

Permite que o frontend sempre trate a resposta pelo campo `Success` antes de acessar `Data`.

#### `DTOs/Matches/`

| DTO | Campos principais | Direção |
|---|---|---|
| `StartMatchResponse` | `MatchId, Quote, Difficulty, AttemptsRemaining, HintsRemaining, Score, IsDaily` | API → Cliente |
| `SubmitGuessRequest` | `GuessText, GuessType` | Cliente → API |
| `GuessResponse` | `IsCorrect, AttemptsRemaining, Score, MatchStatus, Result?` | API → Cliente |
| `HintResponse` | `Hint, HintsRemaining, Score` | API → Cliente |
| `MatchResultResponse` | `BookTitle, Author, Score, XPEarned, Trivia?` | API → Cliente |

#### `UseCases/Auth/`

**`RegisterUseCase`**: valida e-mail único → faz hash BCrypt → cria `User` → gera JWT.

**`LoginUseCase`**: busca usuário por e-mail → verifica hash BCrypt → atualiza `LastLoginAt` → gera JWT.

#### `UseCases/Matches/StartMatchUseCase.cs`

```
1. Verifica se já existe partida ativa → retorna estado atual (idempotente)
2. Se `daily=true`:
   a. Busca DailyChallenge de hoje
   b. Fallback para partida livre se não houver desafio
3. Se `daily=false`: seleciona BookQuote aleatório
4. Cria Match e salva no banco
5. Retorna StartMatchResponse com o trecho e estado inicial
```

#### `UseCases/Matches/SubmitGuessUseCase.cs`

```
1. Valida que a partida existe e pertence ao usuário
2. Verifica que a partida está ativa
3. Normaliza texto (remove acentos, lowercase, trim)
4. Compara com título ou autor conforme GuessType
5. Se correto:
   - RegisterCorrectGuess() → status Won
   - Calcula e credita XP no usuário
   - IncrementStreak()
   - Verifica e concede conquistas
   - Gera trivia via IA (não bloqueia em caso de falha)
6. Se incorreto:
   - RegisterIncorrectGuess() → pode mudar para Lost
   - Se Lost: gera trivia via IA
7. Persiste tudo
8. Retorna GuessResponse
```

A normalização de texto remove acentos para aceitar variações como "Dom Casmurro" / "dom casmurro" / "dóm casmurró":

```csharp
private static string NormalizeText(string text) =>
    text.Trim().ToLowerInvariant()
        .Replace("á","a").Replace("à","a").Replace("ã","a").Replace("â","a")
        .Replace("é","e").Replace("ê","e")
        // ... demais acentos e cedilha
```

#### `UseCases/Matches/RequestHintUseCase.cs`

```
1. Valida partida ativa do usuário
2. Chama Match.UseHint() → decrementa score e verifica limite
3. Chama AIService.GenerateHintAsync() com nível da dica (1, 2 ou 3)
4. Persiste a partida atualizada
5. Retorna HintResponse com o texto da dica
```

#### `UseCases/Profile/GetProfileUseCase.cs`

Busca usuário com conquistas; calcula `TotalWins` (partidas com status Won).

#### `UseCases/Ranking/GetRankingUseCase.cs`

Busca top N usuários ordenados por score total ou XP.

---

### 5.3 `BookGuess.Infrastructure`

#### `Persistence/BookGuessDbContext.cs`

`DbContext` do EF Core. Implementa `IUnitOfWork` (método `SaveChangesAsync`). Configura entidades via `IEntityTypeConfiguration<T>`.

#### `Persistence/DataSeeder.cs`

Executado na inicialização do backend. Idempotente por tabela:

1. **`SeedAchievementsAsync`**: verifica se tabela está vazia; insere 7 conquistas.
2. **`SeedBooksAsync`**: carrega títulos existentes em `HashSet`; insere apenas livros novos — permite adicionar livros sem recriar o banco. Atualmente semeia 11 livros clássicos da literatura brasileira com 3 trechos cada (33 `BookQuotes` no total).
3. **`SeedDailyChallengeAsync`**: semeia desafios para os próximos 31 dias a partir de hoje, sem repetir trechos já alocados na janela.

#### `Persistence/Migrations/`

Única migration `InitialCreate` que cria todas as 9 tabelas com índices e chaves estrangeiras.

#### `Repositories/`

Implementações de `IRepository<T>` usando EF Core:

| Repositório | Métodos notáveis |
|---|---|
| `UserRepository` | `GetByEmailAsync`, `GetByIdAsync` |
| `MatchRepository` | `GetActiveByUserIdAsync` (inclui BookQuote + Book), `GetByIdAsync` |
| `BookQuoteRepository` | `GetRandomAsync` (ORDER BY RAND()) |
| `DailyChallengeRepository` | `GetByDateAsync` (inclui BookQuote + Book) |
| `AchievementRepository` | `GetByCodeAsync`, `UserHasAchievementAsync`, `AddUserAchievementAsync` |

#### `Identity/JwtService.cs`

Gera JWTs com claims `NameIdentifier` (GUID do usuário), `Name` e `Email`. Validade configurável via `Jwt:ExpirationHours`. Usa HMAC-SHA256.

#### `Services/AIServiceClient.cs`

Implementa `IAIService`. Faz chamadas HTTP para o `ai-service` interno:

- `GenerateHintAsync` → `POST /api/ai/hint`
- `GenerateTriviaAsync` → `POST /api/ai/trivia`

---

### 5.4 `BookGuess.Api`

#### `Program.cs`

Pipeline de configuração:

```csharp
// 1. Swagger com suporte a Bearer token
// 2. JWT Bearer authentication (valida issuer, audience, signature, lifetime)
// 3. AddInfrastructure() → DbContext + Repos + JWT + DataSeeder
// 4. AddApplicationUseCases() → todos os Use Cases como Scoped
// 5. HttpClient nomeado "AIService"
// 6. CORS configurado via Cors:AllowedOrigins
// 7. GlobalExceptionMiddleware
// 8. UseCors → UseAuthentication → UseAuthorization → MapControllers
// 9. DataSeeder.SeedAsync() na inicialização
```

#### `Middlewares/GlobalExceptionMiddleware.cs`

Captura qualquer exceção não tratada, loga com `ILogger` e retorna `500` com JSON padronizado `{ success: false, message: "Ocorreu um erro interno." }`. Evita que stack traces vazem para o cliente.

#### `Controllers/`

| Controller | Rota base | Auth | Endpoints |
|---|---|---|---|
| `AuthController` | `/api/auth` | Público | `POST /register`, `POST /login` |
| `MatchesController` | `/api/matches` | `[Authorize]` | `POST /start`, `POST /{id}/guess`, `POST /{id}/hint` |
| `ProfileController` | `/api/profile` | `[Authorize]` | `GET /` |
| `RankingController` | `/api/ranking` | Público | `GET /global`, `GET /weekly`, `GET /monthly` |

Todos os controllers usam **Primary Constructor** do C# 12 para injeção de dependência. O `CurrentUserId` é extraído do JWT via `User.FindFirstValue(ClaimTypes.NameIdentifier)`.

---

### 5.5 `BookGuess.AIService`

Microserviço autônomo com ASP.NET Core Minimal API.

#### `Infrastructure/Claude/ClaudeService.cs`

Wrapper sobre o `AnthropicClient`. Envia prompts ao modelo `claude-haiku-4-5-20251001` com `MaxTokens = 300`. Em caso de erro, loga e relança a exceção.

#### `Infrastructure/Embeddings/EmbeddingService.cs`

Gera vetores de embedding via **Ollama** (modelo `all-minilm`) para uso no pipeline RAG. Chamadas HTTP para `http://ollama:11434`.

#### `Infrastructure/Qdrant/QdrantService.cs`

Interface com o banco vetorial **Qdrant** via gRPC (porta 6334). Responsável por:
- Criar a collection se não existir
- Inserir/atualizar chunks de livros com metadados
- Buscar os 5 chunks mais relevantes por similaridade vetorial para um dado livro

#### `Application/Pipelines/RagPipeline.cs`

Orquestra o fluxo RAG (Retrieval-Augmented Generation):

```
GenerateHintAsync(bookId, hintLevel, ...):
  1. Verifica cache em memória (TTL 6h)
  2. Busca chunks relevantes no Qdrant via embedding da query
  3. Monta prompt com contexto + PromptTemplates.GenerateHint()
  4. Chama Claude API
  5. Armazena resultado no cache
  6. Retorna hint

GenerateTriviaAsync(bookId, ...):
  Idem, TTL 24h

IngestBookAsync(bookId, chunks):
  1. Garante que a collection existe no Qdrant
  2. Gera embedding para cada chunk de texto
  3. Insere vetores no Qdrant com metadados do livro
```

#### `Application/Prompts/PromptTemplates.cs`

Templates de prompt em português para cada tipo de geração:
- `GenerateHint(level, ...)`: nível 1 = dica genérica (gênero/época), nível 2 = mais específica (personagens), nível 3 = muito específica (eventos do enredo)
- `GenerateTrivia(...)`: curiosidade histórica/literária sobre o livro
- `ExplainMistake(...)`: explica por que o palpite estava errado

#### `Endpoints/AiEndpoints.cs`

```
POST /api/ai/hint     → RagPipeline.GenerateHintAsync()
POST /api/ai/trivia   → RagPipeline.GenerateTriviaAsync()
POST /api/ai/explain  → RagPipeline.ExplainMistakeAsync()
POST /api/ai/ingest   → RagPipeline.IngestBookAsync()
GET  /api/ai/health   → { Status: "healthy" }
```

---

### 5.6 `BookGuess.Web`

Frontend Blazor Server. Toda a renderização ocorre no servidor; o browser recebe HTML e mantém um WebSocket SignalR (`/_blazor`) para atualizações em tempo real.

#### `Program.cs`

```csharp
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AppState>();
builder.Services.AddHttpClient<BookGuessApiClient>(c => c.BaseAddress = new Uri(apiBase));
```

`AppState` é **Scoped** (uma instância por circuito/usuário), não Singleton.

#### `Models/AppState.cs`

Serviço de estado global da sessão do usuário. Substitui um Context Provider do React:

```csharp
public class AppState
{
    public string?            Token       { get; set; }  // JWT
    public string?            UserName    { get; set; }
    public int                Level       { get; set; }
    public int                XP          { get; set; }
    public StartMatchResponse? ActiveMatch { get; set; }
    public List<string>       ActiveHints { get; set; } = [];

    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);
    public event Action? OnChange;  // componentes se inscrevem para rerender

    public void SetUser(AuthResponse auth) { /* popula campos + NotifyStateChanged() */ }
    public void Clear()                    { /* logout: zera tudo + NotifyStateChanged() */ }
    public void ClearMatch()               { /* fim de partida */ }
    public void AddHint(string hint)       { /* adiciona dica à lista */ }
    public void UpdateXP(int earned)       { /* credita XP ganho */ }
}
```

**Padrão de uso nos componentes:**

```csharp
@implements IDisposable

protected override void OnInitialized()
{
    AppState.OnChange += HandleAppStateChange;
}

public void Dispose()
{
    _disposed = true;
    AppState.OnChange -= HandleAppStateChange;
}

private void HandleAppStateChange()
{
    if (_disposed) return;
    if (!AppState.IsAuthenticated) { Nav.NavigateTo("/"); return; }
    InvokeAsync(StateHasChanged);
}
```

O `_disposed` flag evita o crash do circuito Blazor quando `AppState.OnChange` dispara após o componente já ter sido descartado (ex: logout durante uma partida ativa).

#### `Services/BookGuessApiClient.cs`

Único ponto de comunicação do frontend com a API. Injeta o `Bearer` token em todas as chamadas autenticadas via `SetAuth()`:

| Método | Endpoint | Auth |
|---|---|---|
| `RegisterAsync` | `POST /api/auth/register` | Não |
| `LoginAsync` | `POST /api/auth/login` | Não |
| `StartMatchAsync(daily)` | `POST /api/matches/start?daily=` | Sim |
| `SubmitGuessAsync` | `POST /api/matches/{id}/guess` | Sim |
| `RequestHintAsync` | `POST /api/matches/{id}/hint` | Sim |
| `GetRankingAsync(tab)` | `GET /api/ranking/{tab}` | Não |
| `GetProfileAsync` | `GET /api/profile` | Sim |

#### `Components/Shared/AuthInitializer.razor`

Componente invisível montado no layout raiz. Na inicialização, lê o token do `localStorage`; se existente, chama `/api/profile` para restaurar o estado do usuário sem exigir novo login.

#### `Components/Shared/Icon.razor`

Componente de ícone SVG inline. Sem CDN externo, sem JavaScript. Renderiza SVG diretamente no HTML com paths do Lucide Icons:

```razor
<svg xmlns="..." width="@Size" height="@Size" viewBox="0 0 24 24"
     fill="@Fill" stroke="currentColor" stroke-width="@StrokeWidth"
     stroke-linecap="round" stroke-linejoin="round" class="lucide @Class">
    @((MarkupString)Path)
</svg>

@code {
    [Parameter] public string Name        { get; set; } = "";
    [Parameter] public int    Size        { get; set; } = 16;
    [Parameter] public string Class       { get; set; } = "";
    [Parameter] public string Fill        { get; set; } = "none";
    [Parameter] public string StrokeWidth { get; set; } = "2";
    private string Path => Name switch { "book-open" => "...", ... };
}
```

#### `Components/Pages/`

| Página | Rota | Descrição |
|---|---|---|
| `Home.razor` | `/` | Landing page (não logado) e dashboard (logado) |
| `Login.razor` | `/login` | Formulário de login com show/hide de senha |
| `Register.razor` | `/registro` | Formulário de registro |
| `Match.razor` | `/partida` | Tela de jogo: exibe trecho, campo de palpite, dicas, pontuação |
| `Ranking.razor` | `/ranking` | Tabela de ranking com abas Global/Semanal/Mensal e pódio |
| `Profile.razor` | `/perfil` | Estatísticas do usuário, barra de XP e conquistas |

#### `Components/Shared/`

| Componente | Função |
|---|---|
| `ResultScreen.razor` | Tela de resultado pós-partida (vitória ou derrota) com animação, trivia e pontuação |
| `AttemptDots.razor` | Indicador visual de tentativas restantes (bolinhas) |
| `DifficultyStars.razor` | Exibe nível de dificuldade com estrelas preenchidas/vazias |
| `RankingRow.razor` | Linha da tabela de ranking com avatar e pontuação |
| `NavBar.razor` | Navbar com grid CSS 3 colunas: logo | stats | ações |

#### `wwwroot/app.css`

Design system completo com variáveis CSS:

```css
:root {
  --bg:           #090C12;   /* fundo escuro */
  --card:         #141928;   /* superfície de cards */
  --primary:      #6D5DFC;   /* roxo principal */
  --secondary:    #8B5CF6;   /* roxo secundário */
  --accent:       #06B6D4;   /* ciano */
  --warning:      #F59E0B;   /* âmbar */
  --success:      #22C55E;   /* verde */
  --danger:       #EF4444;   /* vermelho */
}
```

Inclui: glassmorphism na navbar, animações `float`, `bounce-in`, `slide-up`, `glow-pulse`, `skeleton-shimmer`, utilitários responsivos e o `game-layout` CSS Grid.

---

## 6. Fluxo de Execução

### Inicialização do backend

```
docker compose up backend
  → dotnet run (BookGuess.Api)
  → Program.cs: registra serviços
  → app.Build()
  → GlobalExceptionMiddleware
  → app.Run()
  → DataSeeder.SeedAsync():
      → MigrateAsync() → aplica migrations pendentes
      → SeedAchievementsAsync() → 7 conquistas
      → SeedBooksAsync()        → 11 livros, 33 trechos (idempotente por título)
      → SeedDailyChallengeAsync() → desafios para os próximos 31 dias
  → Servidor HTTP disponível em :8080
```

### Inicialização do frontend

```
docker compose up frontend
  → dotnet run (BookGuess.Web)
  → Program.cs: registra Blazor Server, AppState, HttpClient, LocalStorage
  → Aguarda requisições HTTP na :8080
```

### Fluxo de autenticação

```
1. Usuário preenche login → Login.razor chama BookGuessApiClient.LoginAsync()
2. API: LoginUseCase → verifica senha BCrypt → gera JWT → retorna AuthResponse
3. Frontend: AppState.SetUser(auth) → armazena token em memória
4. Blazored.LocalStorage salva o token no localStorage do browser
5. Na próxima visita: AuthInitializer.razor lê localStorage → restaura sessão sem novo login
6. Logout: remove token do localStorage → AppState.Clear() → NavBar rerenderiza
```

### Fluxo de uma partida

```
1. Home.razor → clique em "Jogar" → BookGuessApiClient.StartMatchAsync()
2. Backend: StartMatchUseCase → cria Match → retorna StartMatchResponse
3. AppState.ActiveMatch = response → Nav.NavigateTo("/partida")
4. Match.razor carrega o trecho da partida ativa

Ciclo de tentativa:
5. Usuário digita palpite → SubmitGuess()
6. BookGuessApiClient.SubmitGuessAsync() → API
7. Backend: SubmitGuessUseCase → normaliza textos → compara
8a. Se correto: Match.Won → XP creditado → trivia da IA → GuessResponse(IsCorrect=true, Result)
8b. Se incorreto: Match.pontuação−100 → pode virar Lost → GuessResponse(IsCorrect=false)
9. Frontend atualiza AttemptDots, Score; se resultado → exibe ResultScreen

Dica:
5b. Usuário clica "Pedir Dica" → RequestHint()
6b. Backend: RequestHintUseCase → Match.UseHint() → Claude gera dica → retorna HintResponse
7b. AppState.AddHint(dica) → dica aparece na lista
```

### Gerenciamento de estado reativo

```
AppState.OnChange (event Action) dispara sempre que:
  - SetUser() / Clear() / ClearMatch() / AddHint() / UpdateXP()

Componentes assinantes:
  - NavBar.razor  → reexibe nível/XP
  - Match.razor   → detecta logout e navega para /
  - Profile.razor → detecta logout e navega para /login

Todos implementam IDisposable e cancelam a assinatura no Dispose()
para evitar crashes do circuito Blazor.
```

---

## 7. Frontend em Detalhe

### Hierarquia de componentes

```
App.razor
└── Routes.razor
    └── MainLayout.razor
        ├── NavBar.razor
        │   └── Icon.razor (×n)
        └── [página atual]
            ├── Home.razor
            │   └── DifficultyStars.razor
            ├── Match.razor
            │   ├── AttemptDots.razor
            │   ├── DifficultyStars.razor
            │   ├── Icon.razor (×n)
            │   └── ResultScreen.razor
            │       └── Icon.razor (×n)
            ├── Ranking.razor
            │   └── RankingRow.razor
            │       └── Icon.razor
            ├── Profile.razor
            │   └── Icon.razor (×n)
            ├── Login.razor
            │   └── Icon.razor (×n)
            └── Register.razor
                └── Icon.razor (×n)
```

### Gerenciamento de estado

Não há Context API nem Redux. O estado global é um serviço `AppState` (Scoped/por circuito) injetado diretamente nos componentes. Comunicação por evento `Action?`.

### Roteamento

Blazor Server usa atributos `@page` nas páginas Razor. Não há arquivo de rotas separado:

```
/          → Home.razor
/login     → Login.razor
/registro  → Register.razor
/partida   → Match.razor
/ranking   → Ranking.razor
/perfil    → Profile.razor
```

### Persistência de sessão

`Blazored.LocalStorage` armazena o JWT sob a chave `bg_token`. `AuthInitializer.razor` (montado em `MainLayout`) restaura a sessão na inicialização do circuito.

### Comunicação com a API

Todo o tráfego HTTP passa pelo `BookGuessApiClient`. O `HttpClient` base aponta para `http://backend:8080` (dentro da rede Docker) ou `http://localhost:5000` em desenvolvimento. Requisições passam pelo Nginx que as encaminha ao backend.

---

## 8. Backend em Detalhe

### Fluxo completo de requisição REST

```
POST /api/matches/{id}/guess
  │
  ├─ Nginx (proxy_pass http://backend)
  ├─ GlobalExceptionMiddleware.InvokeAsync() (try/catch wrapper)
  ├─ UseAuthentication() → valida JWT → ClaimsPrincipal
  ├─ UseAuthorization() → [Authorize] verifica claim
  ├─ MatchesController.Guess(id, request)
  │    └─ CurrentUserId = Guid.Parse(User.FindFirstValue(NameIdentifier))
  │    └─ submitGuessUseCase.ExecuteAsync(id, userId, request, ct)
  │         ├─ matchRepository.GetByIdAsync() → EF Core → MySQL
  │         ├─ Normaliza textos (remove acentos, lowercase)
  │         ├─ match.RegisterCorrectGuess() ou RegisterIncorrectGuess()
  │         ├─ user.AddXP() + CheckAchievements()
  │         ├─ aiServiceClient.GenerateTriviaAsync() → POST /api/ai/trivia
  │         ├─ unitOfWork.SaveChangesAsync()
  │         └─ return ApiResponse<GuessResponse>.Ok(...)
  └─ return Ok(result)
```

### Tratamento de erros

- **Exceções não tratadas**: capturadas por `GlobalExceptionMiddleware`, retorna 500 com mensagem genérica.
- **Erros de negócio**: retornados como `ApiResponse.Fail("mensagem")` com status 400/401/404 conforme o caso.
- **Falhas da IA**: envolvidas em `try/catch` — a partida nunca falha por indisponibilidade do Claude.

### Injeção de dependência

| Lifetime | Serviços |
|---|---|
| Scoped | DbContext, todos os Repositórios, Use Cases, JwtService, AIServiceClient |
| Singleton | (nenhum) |
| Transient | (nenhum) |

---

## 9. Banco de Dados

### Modelagem

```
Books ─────────────────────────────────────────────────────────────────
  Id (PK, GUID)  Title  Author  Genre  Description  Difficulty  PublishedYear  CreatedAt

BookQuotes ─────────────────────────────────────────────────────────────
  Id (PK)  BookId (FK→Books)  Content  Difficulty  SourcePage  CreatedAt

Trivias ─────────────────────────────────────────────────────────────────
  Id (PK)  BookId (FK→Books)  Content  Category  CreatedAt

DailyChallenges ─────────────────────────────────────────────────────────
  Id (PK)  BookQuoteId (FK→BookQuotes)  ChallengeDate (DATE, único por data)  Difficulty

Users ───────────────────────────────────────────────────────────────────
  Id (PK)  Name  Email (UNIQUE)  PasswordHash  AvatarUrl  Level  XP
  CurrentStreak  BestStreak  CreatedAt  LastLoginAt  IsActive

Matches ─────────────────────────────────────────────────────────────────
  Id (PK)  UserId (FK→Users)  BookQuoteId (FK→BookQuotes)  Status
  Attempts  HintsUsed  Score (default 1000)  IsDaily  StartedAt  FinishedAt

Guesses ─────────────────────────────────────────────────────────────────
  Id (PK)  MatchId (FK→Matches)  GuessText  GuessType  IsCorrect  CreatedAt

Achievements ────────────────────────────────────────────────────────────
  Id (PK)  Code (UNIQUE)  Name  Description  Icon  XPReward

UserAchievements ────────────────────────────────────────────────────────
  UserId (PK,FK→Users)  AchievementId (PK,FK→Achievements)  UnlockedAt
```

### Relacionamentos

| Relação | Cardinalidade | Cascade |
|---|---|---|
| Book → BookQuote | 1:N | Delete Cascade |
| Book → Trivia | 1:N | Delete Cascade |
| BookQuote → DailyChallenge | 1:N | Delete Cascade |
| BookQuote → Match | 1:N | Delete Cascade |
| User → Match | 1:N | Delete Cascade |
| Match → Guess | 1:N | Delete Cascade |
| User ↔ Achievement | N:N via UserAchievements | Cascade ambos lados |

### Índices

| Tabela | Coluna | Tipo |
|---|---|---|
| Users | Email | UNIQUE |
| Achievements | Code | UNIQUE |
| BookQuotes | BookId | INDEX |
| Matches | UserId | INDEX |
| Matches | BookQuoteId | INDEX |
| Guesses | MatchId | INDEX |
| Trivias | BookId | INDEX |
| DailyChallenges | BookQuoteId | INDEX |

### Conteúdo seed

| Livro | Autor | Dificuldade | Gênero |
|---|---|---|---|
| O Guarani | José de Alencar | 2 | Romantismo |
| Iracema | José de Alencar | 2 | Romantismo |
| Senhora | José de Alencar | 2 | Romantismo |
| Dom Casmurro | Machado de Assis | 3 | Romance |
| Capitães da Areia | Jorge Amado | 3 | Modernismo |
| O Cortiço | Aluísio Azevedo | 4 | Naturalismo |
| Vidas Secas | Graciliano Ramos | 4 | Modernismo |
| São Bernardo | Graciliano Ramos | 4 | Modernismo |
| Memórias Póstumas de Brás Cubas | Machado de Assis | 5 | Realismo |
| Quincas Borba | Machado de Assis | 5 | Realismo |
| Macunaíma | Mário de Andrade | 5 | Modernismo |

---

## 10. APIs

### Base URL

- Produção (via Nginx): `http://localhost/api`
- Backend direto: `http://localhost:5000/api`
- Swagger UI: `http://localhost:5000/swagger` (apenas ambiente Development)

### Auth

#### `POST /api/auth/register`

Registra um novo usuário.

**Body:**
```json
{ "name": "João Silva", "email": "joao@email.com", "password": "Senha123!" }
```

**Resposta 200:**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGci...",
    "name": "João Silva",
    "email": "joao@email.com",
    "level": 1,
    "xp": 0
  }
}
```

**Resposta 400:** `{ "success": false, "message": "E-mail já cadastrado." }`

---

#### `POST /api/auth/login`

Autentica um usuário existente.

**Body:** `{ "email": "joao@email.com", "password": "Senha123!" }`

**Resposta 200:** mesmo formato do register.

**Resposta 401:** `{ "success": false, "message": "Credenciais inválidas." }`

---

### Matches (requer `Authorization: Bearer <token>`)

#### `POST /api/matches/start?daily=false`

Inicia uma nova partida. Se já houver partida ativa, retorna ela (idempotente).

**Query:** `daily=true` para Desafio Diário, `daily=false` para partida aleatória.

**Resposta 200:**
```json
{
  "success": true,
  "data": {
    "matchId": "3fa85f64-...",
    "quote": "Capitu tinha olhos de ressaca...",
    "difficulty": 3,
    "attemptsRemaining": 5,
    "hintsRemaining": 3,
    "score": 1000,
    "isDaily": false
  }
}
```

---

#### `POST /api/matches/{id}/guess`

Envia um palpite para a partida.

**Body:** `{ "guessText": "Dom Casmurro", "guessType": "Book" }`

`guessType` aceita: `"Book"` (título) ou `"Author"` (autor).

**Resposta 200 (acerto):**
```json
{
  "success": true,
  "data": {
    "isCorrect": true,
    "attemptsRemaining": 4,
    "score": 900,
    "matchStatus": "Won",
    "result": {
      "bookTitle": "Dom Casmurro",
      "author": "Machado de Assis",
      "score": 900,
      "xpEarned": 175,
      "trivia": "Dom Casmurro é considerado o maior romance da literatura brasileira."
    }
  }
}
```

**Resposta 200 (erro):**
```json
{
  "success": true,
  "data": {
    "isCorrect": false,
    "attemptsRemaining": 3,
    "score": 800,
    "matchStatus": "Active",
    "result": null
  }
}
```

**Resposta 200 (derrota — 5 erros):**
```json
{
  "data": {
    "isCorrect": false,
    "attemptsRemaining": 0,
    "matchStatus": "Lost",
    "result": { "bookTitle": "Dom Casmurro", "author": "Machado de Assis", "score": 500, "xpEarned": 0, "trivia": "..." }
  }
}
```

---

#### `POST /api/matches/{id}/hint`

Solicita uma dica gerada por IA. Custa 150 pontos. Máximo de 3 dicas por partida.

**Resposta 200:**
```json
{
  "success": true,
  "data": {
    "hint": "Este livro foi publicado no final do século XIX e é considerado um marco do realismo brasileiro.",
    "hintsRemaining": 2,
    "score": 850
  }
}
```

**Resposta 400:** `{ "success": false, "message": "Limite de dicas atingido." }`

---

### Profile (requer token)

#### `GET /api/profile`

**Resposta 200:**
```json
{
  "success": true,
  "data": {
    "name": "João Silva",
    "email": "joao@email.com",
    "level": 2,
    "xp": 650,
    "totalWins": 7,
    "currentStreak": 3,
    "bestStreak": 5,
    "achievements": [
      { "name": "Primeira Página", "description": "Acertou seu primeiro livro.", "icon": "🏅" }
    ]
  }
}
```

---

### Ranking (público)

#### `GET /api/ranking/global` | `/weekly` | `/monthly`

**Resposta 200:**
```json
{
  "success": true,
  "data": [
    { "name": "Maria Santos", "score": 4200, "level": 4 },
    { "name": "João Silva",   "score": 3100, "level": 3 }
  ]
}
```

> **Nota:** Atualmente os três endpoints (`global`, `weekly`, `monthly`) retornam o mesmo ranking geral. A diferenciação por período é uma melhoria futura.

---

### AI Service (interno, não exposto via Nginx)

| Endpoint | Body | Descrição |
|---|---|---|
| `POST /api/ai/hint` | `{ bookId, hintLevel, title, author, quote }` | Gera dica contextual |
| `POST /api/ai/trivia` | `{ bookId, title, author }` | Gera curiosidade literária |
| `POST /api/ai/explain` | `{ bookId, incorrectGuess }` | Explica por que o palpite estava errado |
| `POST /api/ai/ingest` | `{ bookId, title, author, chunks[] }` | Ingere chunks no Qdrant |
| `GET /api/ai/health` | — | Healthcheck |

---

## 11. Segurança

### Autenticação

- **JWT HS256**: gerado com segredo mínimo de 32 caracteres, com validação de issuer, audience, assinatura e expiração. `ClockSkew = TimeSpan.Zero` evita tolerância de tempo.
- **Token no localStorage**: suficiente para SPAs; o Blazor Server mantém o estado em memória no servidor e usa o localStorage apenas para persistir entre recargas.

### Senhas

- Hash com **BCrypt** (salt automático, fator de custo padrão ≈ 10).
- Senha nunca trafega ou é armazenada em texto simples.

### Autorização

- Endpoints de matches e perfil exigem `[Authorize]`.
- O `CurrentUserId` é sempre extraído do JWT, nunca do body da requisição, evitando IDOR.

### CORS

- Configurado para aceitar apenas origens específicas via `Cors:AllowedOrigins` (ex: `http://frontend:80`).

### Tratamento de erros

- `GlobalExceptionMiddleware` nunca expõe stack traces ao cliente.
- Respostas de erro têm formato padronizado sem revelar detalhes internos.

### Variáveis de ambiente

- Segredos (`CLAUDE_API_KEY`, `JWT_SECRET`, `MYSQL_PASSWORD`) estão no `.env` que **não é commitado**.
- O `.env.example` documenta os campos necessários sem os valores reais.

### Pontos de atenção

- O endpoint `POST /api/ai/ingest` não possui autenticação no AI Service (apenas rede interna Docker o acessa).
- O ranking global não requer autenticação, o que é intencional para permitir visualização pública.

---

## 12. Configuração e Infraestrutura

### Variáveis de Ambiente (`.env`)

| Variável | Descrição | Exemplo |
|---|---|---|
| `CLAUDE_API_KEY` | Chave da API Anthropic | `sk-ant-...` |
| `JWT_SECRET` | Segredo para assinar JWTs (mín. 32 chars) | `meu-segredo-super-secreto-32chars` |
| `MYSQL_ROOT_PASSWORD` | Senha root do MySQL | `root-senha-forte` |
| `MYSQL_PASSWORD` | Senha do usuário `bookguess` no MySQL | `senha-do-app` |

### Docker Compose

6 serviços na rede interna `bookguess` (bridge):

```
┌────────────────┬──────────────┬──────────────────────────────┐
│ Serviço        │ Porta host   │ Depende de                   │
├────────────────┼──────────────┼──────────────────────────────┤
│ nginx          │ 80, 443      │ backend, frontend             │
│ backend        │ 5000→8080    │ mysql (healthcheck)           │
│ frontend       │ (interno)    │ backend                       │
│ ai-service     │ (interno)    │ qdrant                        │
│ mysql          │ (interno)    │ —                             │
│ qdrant         │ (interno)    │ —                             │
└────────────────┴──────────────┴──────────────────────────────┘
```

O MySQL só é considerado pronto quando responde ao `mysqladmin ping` (healthcheck). O backend aguarda esse healthcheck antes de iniciar.

### Nginx

Três location blocks:

| Location | Destino | Propósito |
|---|---|---|
| `/api/` | `backend:8080` | REST API |
| `/_blazor` | `frontend:8080` | WebSocket SignalR (Connection: Upgrade) |
| `/` | `frontend:8080` | Blazor Server (HTML + assets) |

### Como rodar localmente

```bash
# 1. Clone o repositório
git clone https://github.com/BragaNux/book-guess.git
cd book-guess

# 2. Configure as variáveis de ambiente
cp .env.example .env
# Edite .env com suas chaves reais

# 3. Suba todos os containers
docker compose up -d

# 4. Acesse no browser
# http://localhost
```

O seeder roda automaticamente na inicialização do backend e popula o banco com livros, trechos, conquistas e 31 desafios diários.

### Builds Docker

Todos os Dockerfiles seguem o padrão **multi-stage build**:
1. Stage `build`: imagem `sdk` → `dotnet publish -c Release`
2. Stage `final`: imagem `aspnet` (runtime apenas) → copia o publish

Isso garante imagens de produção menores, sem o SDK.

---

## 13. Dependências do Projeto

### `BookGuess.Api`

| Pacote | Versão | Uso |
|---|---|---|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 9.x | Middleware de validação de JWT |
| `Microsoft.IdentityModel.Tokens` | 8.x | `SymmetricSecurityKey`, `TokenValidationParameters` |
| `Swashbuckle.AspNetCore` | 7.x | Swagger/OpenAPI UI |

### `BookGuess.Infrastructure`

| Pacote | Versão | Uso |
|---|---|---|
| `Microsoft.EntityFrameworkCore` | 9.x | ORM base |
| `Pomelo.EntityFrameworkCore.MySql` | 9.x | Provider MySQL para EF Core |
| `BCrypt.Net-Next` | 4.x | Hash de senhas |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 9.x | Geração de JWTs via `JwtSecurityTokenHandler` |

### `BookGuess.AIService`

| Pacote | Versão | Uso |
|---|---|---|
| `Anthropic.SDK` | 3.x | Cliente oficial da API Claude |
| `Qdrant.Client` | 1.x | gRPC client para o Qdrant |
| `Microsoft.Extensions.Caching.Memory` | 9.x | Cache in-process de hints e trivias |

### `BookGuess.Web`

| Pacote | Versão | Uso |
|---|---|---|
| `Blazored.LocalStorage` | 4.x | Acesso ao localStorage do browser via JS interop |

---

## 14. Regras de Negócio

### Partida

| Regra | Detalhe |
|---|---|
| Máximo de tentativas | 5 por partida |
| Máximo de dicas | 3 por partida |
| Pontuação inicial | 1000 pontos |
| Penalidade por erro | −100 pontos |
| Penalidade por dica | −150 pontos |
| Pontuação mínima | 0 (não vai negativo) |
| Partida ativa única | Usuário não pode iniciar nova partida se já houver uma ativa |
| Modo diário | Um trecho fixo por dia, igual para todos os jogadores |
| Comparação insensível | Acentos e capitalização ignorados na comparação de palpites |
| Tipo de palpite | Pode ser título (`Book`) ou autor (`Author`) |

### XP e Níveis

| Ação | XP ganho |
|---|---|
| Vitória base | 100 XP |
| Bônus: acertou na 1ª tentativa | +50 XP |
| Bônus: sem usar dicas | +25 XP |
| Bônus: Desafio Diário | +100 XP |
| Conquistas desbloqueadas | XP variável (10–1000) |

| Nível | XP necessário |
|---|---|
| 1 | 0–499 |
| 2 | 500–999 |
| 3 | 1.000–1.999 |
| 4 | 2.000–3.499 |
| 5 | 3.500+ |

### Conquistas

| Código | Nome | Condição | XP |
|---|---|---|---|
| `FIRST_WIN` | Primeira Página | Primeira vitória | 50 |
| `FIRST_HINT` | Preciso de Ajuda | Primeiro uso de dica | 10 |
| `TEN_BOOKS` | Leitor Iniciante | 10 vitórias totais | 100 |
| `FIFTY_BOOKS` | Explorador Literário | 50 vitórias | 250 |
| `HUNDRED_BOOKS` | Bibliotecário | 100 vitórias | 500 |
| `FIVE_HUNDRED_BOOKS` | Guardião dos Clássicos | 500 vitórias | 1.000 |
| `SEVEN_DAY_STREAK` | Consistência | 7 dias consecutivos de jogo | 150 |

### Desafios Diários

- Um trecho é associado a cada data; todos os jogadores que jogar naquele dia enfrentam o mesmo trecho.
- O DataSeeder pre-semeia os próximos **31 dias** sem repetição de trechos.
- Se não houver desafio cadastrado para o dia (ex: após 31 dias sem reiniciar o backend), o sistema faz fallback para partida livre.

### Sequência (Streak)

- Incrementada a cada vitória.
- **Não é resetada automaticamente** quando o usuário perde um dia — a lógica de reset diário está prevista na entidade mas não há job agendado implementado ainda.

### Dicas com IA

- Dica de nível 1: vaga (época, gênero literário).
- Dica de nível 2: intermediária (personagens, contexto).
- Dica de nível 3: específica (eventos do enredo, estilo do autor).
- Cache de 6h por livro/nível: o mesmo hint não regenera desnecessariamente.
- Falha na IA **nunca** bloqueia o fluxo da partida.

---

## 15. Resumo Executivo

### O que é

BookGuess é um jogo web de adivinhação literária com foco em clássicos da literatura brasileira. Combina mecânicas de jogo diário (estilo Wordle) com geração de conteúdo por IA (Claude) para criar uma experiência educativa e viciante.

### Principais tecnologias

`.NET 9` · `ASP.NET Core 9` · `Blazor Server` · `EF Core 9` · `MySQL 8` · `Claude Haiku` · `Qdrant` · `Docker`

### Principais módulos

| Módulo | Responsabilidade |
|---|---|
| API REST | Autenticação, partidas, ranking, perfil |
| AI Service | Dicas e trivia via Claude com cache e RAG |
| Frontend Blazor | UI reativa sem JavaScript, estado gerenciado por AppState |
| DataSeeder | Popula 11 livros, 33 trechos e 31 desafios diários na inicialização |

### Fluxo principal

```
Login → Escolhe modo → Lê trecho → Palpita (até 5×) → [Dica da IA] → Resultado + Trivia → XP + Conquistas
```

### Pontos fortes

- **Zero JavaScript customizado**: toda a lógica de UI em C# via Blazor Server.
- **IA degradável graciosamente**: falhas no Claude não quebram o jogo.
- **DataSeeder idempotente por título**: novos livros podem ser adicionados sem recriar o banco.
- **Ícones sem CDN**: `Icon.razor` renderiza SVG Lucide inline, sem dependência externa.
- **Circuitos Blazor estáveis**: padrão `IDisposable + _disposed + OnChange` evita crashes em navegações assíncronas.

### Pontos de atenção

- **Streak sem job agendado**: o reset de sequência por dia perdido não está automatizado; requer um background service (Quartz.NET / Hangfire) ou trigger agendado.
- **Ranking semanal/mensal não implementado**: os três endpoints retornam o mesmo ranking global; a filtragem por período ainda não existe.
- **Qdrant vazio**: o pipeline RAG está implementado, mas o `IngestBookAsync` precisa ser chamado para popular o Qdrant com os trechos dos livros. Sem ingestão, as dicas são geradas apenas com o prompt base.
- **Testes limitados**: apenas testes de domínio (`MatchTests`, `UserTests`). Faltam testes de integração para Use Cases e controllers.

### Recomendações futuras

1. **Job de streak diário**: `IHostedService` ou Hangfire para resetar streaks de usuários que não jogaram no dia anterior.
2. **Ranking diferenciado**: adicionar campo `CreatedAt` nas consultas de ranking para filtrar por semana/mês.
3. **Ingestão automática**: chamar `POST /api/ai/ingest` no DataSeeder após seed de cada livro para popular o Qdrant.
4. **Testes de integração**: WebApplicationFactory para testar o pipeline completo HTTP → Use Case → DB.
5. **Rate limiting**: proteger endpoints de autenticação contra força bruta.
6. **Refresh token**: o JWT atual é de uso único sem renovação; implementar refresh token para melhor UX.
7. **Mais livros**: o seeder é facilmente extensível — adicionar mais autores e épocas aumenta a variedade dos desafios mensais.
