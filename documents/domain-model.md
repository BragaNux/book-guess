# BookGuess - Domain Model

# Objetivo

Este documento define o modelo de domínio oficial do sistema.

Todas as regras de negócio, entidades, relacionamentos e fluxos devem respeitar este documento.

Em caso de conflito entre implementações e este documento, o domínio possui prioridade.

---

# Visão Geral do Domínio

BookGuess é uma plataforma de gamificação literária onde usuários recebem trechos de livros e tentam descobrir:

* O nome da obra
* O autor

O sistema recompensa conhecimento literário através de:

* XP
* Níveis
* Streaks
* Conquistas
* Ranking

A Inteligência Artificial auxilia o usuário através de dicas contextuais.

---

# Aggregate Root

## User

Usuário é o principal Aggregate Root do sistema.

É responsável por:

* Progresso
* XP
* Conquistas
* Partidas
* Ranking

---

# Entidades

## User

Representa um jogador da plataforma.

### Campos

```text id="nsvxxn"
Id

Name

Email

PasswordHash

AvatarUrl

Level

XP

CurrentStreak

BestStreak

CreatedAt

LastLoginAt

IsActive
```

---

### Regras

```text id="epv5sk"
XP nunca pode ser negativo

Level nunca pode ser menor que 1

CurrentStreak nunca pode ser negativo

BestStreak nunca pode ser menor que CurrentStreak
```

---

### Relacionamentos

```text id="yidnks"
User
 ├── Matches
 ├── Achievements
 └── DailyChallenges
```

---

# Book

Representa uma obra literária.

---

### Campos

```text id="6fxtpq"
Id

Title

Author

Genre

Description

Difficulty

PublishedYear

CreatedAt
```

---

### Regras

```text id="m83d4l"
Título obrigatório

Autor obrigatório

Difficulty entre 1 e 5
```

---

### Relacionamentos

```text id="yfzsmb"
Book
 ├── Quotes
 ├── Trivia
 └── RAG Chunks
```

---

# BookQuote

Representa um trecho utilizado durante o jogo.

---

### Campos

```text id="m6rjpj"
Id

BookId

Content

Difficulty

SourcePage

CreatedAt
```

---

### Regras

```text id="gohjot"
Trecho obrigatório

Trecho deve pertencer a um livro

Difficulty entre 1 e 5
```

---

### Relacionamentos

```text id="qk4lwt"
BookQuote
 └── Book
```

---

# Match

Representa uma partida.

---

### Campos

```text id="0u1jxg"
Id

UserId

BookQuoteId

Status

Attempts

HintsUsed

Score

StartedAt

FinishedAt
```

---

### Status

```text id="1svx6m"
Active

Won

Lost

Abandoned
```

---

### Regras

```text id="2q7y75"
Attempts >= 0

HintsUsed >= 0

Score >= 0
```

---

### Relacionamentos

```text id="cr1l8m"
Match
 ├── User
 ├── BookQuote
 └── Guesses
```

---

# Guess

Representa uma tentativa realizada pelo usuário.

---

### Campos

```text id="0vd3o2"
Id

MatchId

GuessText

GuessType

IsCorrect

CreatedAt
```

---

### GuessType

```text id="okhk3m"
Book

Author
```

---

### Relacionamentos

```text id="5ehj6v"
Guess
 └── Match
```

---

# Achievement

Representa uma conquista desbloqueável.

---

### Campos

```text id="nn8v5z"
Id

Code

Name

Description

Icon

XPReward
```

---

### Exemplos

```text id="xfp1hm"
FIRST_WIN

FIRST_HINT

TEN_BOOKS

FIFTY_BOOKS

HUNDRED_BOOKS

SEVEN_DAY_STREAK
```

---

# UserAchievement

Representa uma conquista desbloqueada.

---

### Campos

```text id="ecnhwl"
UserId

AchievementId

UnlockedAt
```

---

### Relacionamentos

```text id="x6wim8"
UserAchievement
 ├── User
 └── Achievement
```

---

# DailyChallenge

Representa o desafio diário.

Todos os usuários recebem o mesmo desafio.

---

### Campos

```text id="lx6d8n"
Id

BookQuoteId

ChallengeDate

Difficulty
```

---

### Regras

```text id="2ogthd"
Apenas um desafio por dia

Todos os usuários recebem o mesmo desafio
```

---

# Trivia

Representa informações complementares sobre uma obra.

Utilizado pelo sistema RAG.

---

### Campos

```text id="wbzj4z"
Id

BookId

Content

Category

CreatedAt
```

---

### Exemplos

```text id="dbi3gd"
Curiosidade

Contexto Histórico

Personagens

Influência Cultural
```

---

# Value Objects

## XP

Representa experiência acumulada.

---

### Regras

```text id="6h2zht"
Nunca negativo
```

---

## Level

Representa nível do jogador.

---

### Regras

```text id="oqlhsl"
Mínimo 1
```

---

## Streak

Representa sequência diária.

---

### Regras

```text id="vb6jdy"
Mínimo 0
```

---

# Regras de XP

## Acerto

```text id="tyhfdq"
+100 XP
```

---

## Primeira Tentativa

```text id="sgdcj8"
+50 XP
```

---

## Sem Dicas

```text id="x7t74z"
+25 XP
```

---

## Desafio Diário

```text id="z0phg6"
+100 XP bônus
```

---

# Regras de Pontuação

Base:

```text id="2t0g8q"
1000 pontos
```

---

Penalidades

Cada erro:

```text id="f0c4v8"
-100 pontos
```

---

Cada dica:

```text id="0r7zlc"
-150 pontos
```

---

Pontuação mínima:

```text id="d9l4sp"
0
```

---

# Regras de Progressão

## Nível

Fórmula inicial:

```text id="aic7zy"
Nível 1 = 0 XP

Nível 2 = 500 XP

Nível 3 = 1000 XP

Nível 4 = 2000 XP

Nível 5 = 3500 XP
```

---

# Regras de Streak

O streak aumenta quando:

```text id="a4hs2d"
Usuário conclui ao menos uma partida por dia
```

---

O streak é perdido quando:

```text id="eh3g7u"
24 horas sem atividade
```

---

# Limites da Partida

Máximo de tentativas:

```text id="jlwm1n"
5
```

---

Máximo de dicas:

```text id="ktziv0"
3
```

---

# Domínio da IA

A IA nunca participa diretamente das regras de negócio.

A IA apenas fornece:

```text id="v3lxn0"
Dicas

Curiosidades

Explicações

Resumos
```

---

A IA nunca decide:

```text id="htb16z"
Pontuação

XP

Nível

Ranking

Vitória

Derrota
```

---

# Relacionamento Geral

```text id="94j2o7"
User
 │
 ├── Match
 │      │
 │      ├── Guess
 │      │
 │      └── BookQuote
 │                │
 │                └── Book
 │
 ├── UserAchievement
 │
 └── DailyChallenge


Book
 ├── BookQuote
 ├── Trivia
 └── RAG Chunks
```

---

# Regra Fundamental

O domínio é a fonte da verdade.

Frontend, Backend, Banco de Dados e IA devem refletir este modelo.

Nenhuma implementação pode introduzir conceitos fora deste domínio sem atualização prévia deste documento.
