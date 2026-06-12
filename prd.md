# BookGuess - Product Requirements Document (PRD)

Version: 1.0

Status: MVP

---

# Visão Geral

BookGuess é uma plataforma de gamificação literária onde os usuários recebem trechos de livros e precisam descobrir:

* O nome da obra
* O autor

A experiência é enriquecida por Inteligência Artificial, que fornece dicas contextuais, curiosidades e explicações.

O objetivo é transformar conhecimento literário em uma experiência divertida, competitiva e educativa.

---

# Problema

Pessoas que gostam de literatura possuem poucas formas modernas e interativas de testar seu conhecimento.

Os formatos existentes normalmente são:

* Questionários estáticos
* Sites antigos
* Experiências pouco engajadoras

Existe uma oportunidade de criar uma experiência semelhante a:

* Wordle
* Contexto
* GeoGuessr

Mas focada em livros.

---

# Objetivo do Produto

Criar uma plataforma que:

* Incentive leitura
* Reforce conhecimento literário
* Promova competição saudável
* Utilize IA para enriquecer a experiência
* Seja acessível via navegador

---

# Público-Alvo

## Primário

Leitores frequentes.

Faixa etária:

```text
16 a 45 anos
```

---

## Secundário

* Estudantes
* Professores
* Clubes de leitura
* Universitários
* Pessoas interessadas em literatura

---

# Proposta de Valor

BookGuess permite que usuários descubram livros através de seus próprios trechos, transformando leitura em um desafio diário gamificado.

---

# MVP

O MVP deve ser pequeno, funcional e completo.

---

# Funcionalidades Obrigatórias

## Autenticação

Usuário pode:

* Criar conta
* Fazer login
* Encerrar sessão

---

## Jogar

Usuário recebe:

* Um trecho de livro

O usuário pode tentar descobrir:

* Livro
* Autor

---

## Sistema de Tentativas

Usuário possui limite de tentativas por partida.

Cada tentativa é registrada.

---

## Sistema de Dicas com IA

Usuário pode solicitar dicas.

A IA gera dicas progressivas.

Quanto mais dicas:

* Menor pontuação

---

## Resultado da Partida

Ao finalizar:

Exibir:

* Livro correto
* Autor correto
* Pontuação
* XP recebido
* Curiosidade gerada pela IA

---

## Perfil

Exibir:

* Nível
* XP
* Streak
* Estatísticas

---

## Ranking

Exibir:

* Ranking Global
* Ranking Semanal
* Ranking Mensal

---

## Conquistas

Usuário desbloqueia badges ao atingir objetivos.

---

## Desafio Diário

Todos os usuários recebem o mesmo desafio do dia.

---

# Funcionalidades da IA

## Gerar Dica

Entrada:

```text
Partida atual
```

Saída:

```text
Dica contextual
```

---

## Gerar Curiosidade

Entrada:

```text
Livro descoberto
```

Saída:

```text
Curiosidade sobre a obra
```

---

## Explicar Erro

Entrada:

```text
Resposta incorreta
```

Saída:

```text
Explicação contextual
```

---

## Resumo de Livro

Entrada:

```text
Livro
```

Saída:

```text
Resumo curto
```

---

# Fluxo Principal

```text
Login

↓

Iniciar Partida

↓

Receber Trecho

↓

Responder

↓

Acertou?
 ├─ Sim
 │    ↓
 │ Resultado
 │
 └─ Não
      ↓
 Nova Tentativa
      ↓
 Pedir Dica
      ↓
 Resultado
```

---

# Jornada do Usuário

## Primeiro Acesso

Usuário:

```text
Cadastro
↓
Tutorial rápido
↓
Primeira partida
↓
Primeiro XP
↓
Primeira conquista
```

Objetivo:

Criar engajamento imediato.

---

## Usuário Recorrente

```text
Login
↓
Desafio Diário
↓
Partidas
↓
XP
↓
Ranking
```

Objetivo:

Criar hábito diário.

---

# Regras de Negócio

## Tentativas

Máximo:

```text
5
```

---

## Dicas

Máximo:

```text
3
```

---

## XP

Acerto:

```text
100 XP
```

---

Primeira tentativa:

```text
50 XP bônus
```

---

Sem dicas:

```text
25 XP bônus
```

---

Desafio diário:

```text
100 XP bônus
```

---

# Sistema de Níveis

Exemplo:

```text
Nível 1 → 0 XP

Nível 2 → 500 XP

Nível 3 → 1000 XP

Nível 4 → 2000 XP

Nível 5 → 3500 XP
```

---

# Conquistas

## Primeira Página

```text
Primeiro acerto
```

---

## Leitor Iniciante

```text
10 livros descobertos
```

---

## Explorador Literário

```text
50 livros descobertos
```

---

## Bibliotecário

```text
100 livros descobertos
```

---

## Guardião dos Clássicos

```text
500 livros descobertos
```

---

## Consistência

```text
7 dias de streak
```

---

# Ranking

Critério principal:

```text
Pontuação Total
```

Critério secundário:

```text
XP
```

---

# Dados Iniciais

O MVP deve iniciar com:

```text
50 a 100 livros
```

---

Cada livro deve possuir:

```text
Descrição

Trechos

Curiosidades

Autor
```

---

# Escopo Técnico

## Frontend

Tecnologia:

```text
Blazor Web App (.NET 8)
```

---

## Backend

Tecnologia:

```text
ASP.NET Core 8
```

---

## Banco

Tecnologia:

```text
MySQL
```

---

## IA

Tecnologia:

```text
Claude 3.5 Haiku
```

---

## RAG

Tecnologia:

```text
Qdrant
```

---

## Infraestrutura

Tecnologia:

```text
Docker
Docker Compose
Nginx
Ubuntu
```

---

# Métricas de Sucesso

## Produto

Usuário consegue:

* Criar conta
* Jogar
* Solicitar dica
* Acertar livro
* Ganhar XP
* Aparecer no ranking

Sem auxílio manual.

---

## Técnico

Sistema publicado e acessível por URL pública.

---

## Acadêmico

Demonstrar:

* Desenvolvimento Full Stack
* Arquitetura de Software
* Inteligência Artificial
* RAG
* Banco Vetorial
* Docker
* Deploy

---

# Fora do Escopo (MVP)

Não implementar:

* Chat entre usuários
* Multiplayer em tempo real
* Marketplace
* Sistema de amigos
* Aplicativo mobile nativo
* Pagamentos
* Assinaturas
* Rede social
* Upload público de livros

---

# Possíveis Evoluções Futuras

* Aplicativo Mobile
* Sistema de Temporadas
* Torneios
* Modo Competitivo
* Clubes de Leitura
* Recomendação de Livros com IA
* Agentes Literários Especializados
* Multiplayer
* Compartilhamento de Resultados

---

# Definição de Pronto

O projeto será considerado concluído quando:

✅ Usuário consegue criar conta

✅ Usuário consegue jogar

✅ IA gera dicas

✅ IA gera curiosidades

✅ Ranking funciona

✅ Sistema de XP funciona

✅ Conquistas funcionam

✅ Sistema está publicado em URL pública

✅ Aplicação roda via Docker Compose

✅ Demonstração pode ser realizada ao vivo sem intervenção manual

---

# Visão Final

BookGuess deve parecer um produto SaaS moderno de gamificação literária, utilizando Inteligência Artificial e RAG para criar uma experiência divertida, educativa e tecnicamente robusta.
