# BookGuess - Agent Rules

# Objetivo

Este documento define as regras obrigatórias para todos os agentes de IA que contribuírem para o projeto BookGuess.

O objetivo é garantir:

* Consistência
* Qualidade
* Manutenibilidade
* Simplicidade
* Padronização

Todos os agentes devem seguir estas regras sem exceção.

---

# Prioridades

Ao tomar qualquer decisão técnica, seguir esta ordem:

1. Simplicidade
2. Legibilidade
3. Manutenibilidade
4. Performance
5. Escalabilidade

Nunca sacrificar simplicidade por abstrações desnecessárias.

---

# Filosofia do Projeto

BookGuess é um projeto acadêmico com qualidade de produto real.

O objetivo não é demonstrar complexidade.

O objetivo é demonstrar:

* Boa arquitetura
* Boa engenharia
* Boa experiência do usuário
* Integração com IA

Sempre preferir soluções simples e robustas.

---

# Stack Oficial

## Frontend

```text
Blazor Web App
.NET 8
```

---

## Backend

```text
ASP.NET Core 8
```

---

## Banco Relacional

```text
MySQL 8
```

---

## Banco Vetorial

```text
Qdrant
```

---

## IA

```text
Claude 3.5 Haiku
```

---

## Infraestrutura

```text
Docker
Docker Compose
Nginx
Ubuntu
```

---

# Regras Gerais

Sempre:

* Escrever código limpo
* Escrever código legível
* Utilizar nomes claros
* Manter baixo acoplamento
* Criar componentes reutilizáveis
* Seguir os documentos oficiais do projeto

Nunca:

* Adicionar dependências sem necessidade
* Criar abstrações prematuras
* Implementar funcionalidades fora do PRD
* Alterar regras de negócio sem autorização
* Introduzir complexidade desnecessária

---

# Fonte da Verdade

Os agentes devem seguir esta prioridade:

```text
1. agent-rules.md

2. prd.md

3. domain-model.md

4. backend-architecture.md

5. ai-architecture.md

6. frontend-guidelines.md
```

Em caso de conflito:

O documento com prioridade maior vence.

---

# Escopo do MVP

Implementar apenas:

* Cadastro
* Login
* Perfil
* Partidas
* Dicas
* IA
* Ranking
* Conquistas
* Desafio Diário

Não implementar nada além disso.

---

# Funcionalidades Proibidas

Não implementar:

* Chat
* Amigos
* Multiplayer
* Marketplace
* Pagamentos
* Assinaturas
* Notificações Push
* Feed Social
* Upload público de livros
* Administração avançada

Sem autorização explícita.

---

# Regras Frontend

Sempre:

* Seguir frontend-guidelines.md
* Priorizar UX
* Priorizar acessibilidade
* Criar componentes reutilizáveis
* Manter responsividade

Nunca:

* Duplicar componentes
* Criar CSS inline excessivo
* Ignorar estados de loading
* Ignorar estados de erro

---

# Regras Backend

Sempre:

* Seguir Clean Architecture simplificada
* Utilizar Dependency Injection
* Utilizar DTOs
* Centralizar regras de negócio

Nunca:

* Colocar regra de negócio em Controllers
* Acessar banco diretamente da API
* Duplicar regras de pontuação
* Misturar responsabilidades

---

# Regras de Banco de Dados

Sempre:

* Utilizar migrations
* Utilizar chaves estrangeiras
* Utilizar índices adequados

Nunca:

* Utilizar SELECT *
* Duplicar dados desnecessariamente
* Ignorar relacionamentos definidos no domínio

---

# Regras da IA

Sempre:

* Utilizar RAG
* Recuperar contexto antes da geração
* Utilizar prompts centralizados
* Registrar métricas

Nunca:

* Chamar Claude diretamente do Frontend
* Ignorar contexto recuperado
* Hardcodar prompts em serviços
* Expor API Keys

---

# Regras de Segurança

Sempre:

* Utilizar JWT
* Utilizar BCrypt
* Utilizar variáveis de ambiente
* Validar entradas do usuário

Nunca:

* Armazenar senhas em texto puro
* Expor segredos
* Confiar em dados do cliente
* Retornar stack traces para usuários

---

# Regras de API

Sempre:

* Utilizar REST
* Retornar códigos HTTP corretos
* Utilizar versionamento se necessário
* Padronizar respostas

Exemplo:

```json
{
  "success": true,
  "data": {}
}
```

ou

```json
{
  "success": false,
  "message": "Erro descritivo"
}
```

---

# Regras de Código

Preferir:

```csharp
var user = await repository.GetByIdAsync(id);
```

ao invés de:

```csharp
var x = await repository.GetByIdAsync(id);
```

---

Nomes devem ser explícitos.

Evitar abreviações.

---

# Regras de Comentários

Comentar apenas quando necessário.

Evitar:

```csharp
// Incrementa contador
counter++;
```

Preferir código autoexplicativo.

---

# Regras de Testes

Criar testes para:

* XP
* Ranking
* Progressão
* Conquistas
* Regras de partida

Não criar testes triviais.

---

# Regras de Docker

Cada serviço deve possuir:

```text
Dockerfile
```

---

Todo ambiente deve subir com:

```bash
docker compose up -d
```

Sem dependências manuais.

---

# Regras de Performance

Priorizar:

* Simplicidade
* Clareza

Somente otimizar após necessidade comprovada.

Não implementar otimizações prematuras.

---

# Regras de Observabilidade

Sempre registrar:

* Erros
* Tempo de execução
* Chamadas para IA

Nunca registrar:

* Senhas
* Tokens
* API Keys

---

# Regras de Dependências

Antes de adicionar uma biblioteca:

Perguntar:

```text
Isso realmente resolve um problema que não conseguimos resolver com a stack atual?
```

Se a resposta for não:

Não adicionar.

---

# Critério de Aceitação

Uma implementação é considerada aceita quando:

* Compila
* Funciona
* Segue o domínio
* Segue o PRD
* Segue a arquitetura
* Mantém simplicidade

---

# Regra Fundamental

Sempre desenvolver como se o projeto fosse evoluir para um produto real.

Porém, nunca adicionar complexidade que não seja necessária para o MVP atual.

A melhor solução é a mais simples que resolve corretamente o problema.
