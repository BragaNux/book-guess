# BookGuess - Frontend Architecture, Design System & UX Guidelines

## Objetivo

Construir uma experiência de usuário premium para uma plataforma de gamificação literária baseada em Inteligência Artificial.

O produto deve transmitir a sensação de um SaaS moderno pronto para produção, não de um projeto acadêmico.

Toda decisão de interface deve priorizar:

* Clareza
* Engajamento
* Gamificação
* Performance
* Elegância
* Acessibilidade

---

# Visão do Produto

BookGuess é um jogo onde usuários recebem trechos, descrições ou pistas sobre livros e devem descobrir:

* O nome da obra
* O autor

A IA auxilia através de dicas inteligentes e explicações contextuais.

O objetivo principal da interface é incentivar sessões longas de uso e retorno diário.

---

# Princípios de Design

## Gamificação Primeiro

Toda tela deve reforçar:

* Progresso
* Conquistas
* Descoberta
* Recompensa

O usuário nunca deve sentir que está preenchendo formulários.

Ele deve sentir que está jogando.

---

## Menos Interface, Mais Conteúdo

O trecho do livro é o elemento principal.

Todo o restante deve servir para apoiar a experiência.

Evitar:

* Menus complexos
* Telas poluídas
* Informações redundantes

---

## Feedback Imediato

Toda ação do usuário deve gerar feedback visual.

Exemplos:

* Hover
* Focus
* Loading
* Acerto
* Erro
* Evolução de nível

Nenhuma ação deve parecer ignorada pelo sistema.

---

## Mobile First

A experiência deve ser excelente em:

* Smartphones
* Tablets
* Desktop

Todo layout deve ser projetado inicialmente para dispositivos móveis.

---

# Referências

Utilizar como inspiração:

* Duolingo
* Contexto
* Wordle
* Goodreads
* Chess.com
* Linear
* Notion
* Spotify

Misturar:

* Sofisticação literária
* Gamificação moderna
* SaaS premium

---

# Identidade Visual

## Tema Principal

Dark Mode.

O tema escuro será a experiência padrão.

Tema claro pode ser implementado futuramente.

---

## Paleta

Background:

```css
#0F1117
```

Surface:

```css
#171A21
```

Card:

```css
#1D2330
```

Primary:

```css
#6D5DFC
```

Secondary:

```css
#8B5CF6
```

Success:

```css
#22C55E
```

Warning:

```css
#F59E0B
```

Danger:

```css
#EF4444
```

Text Primary:

```css
#FFFFFF
```

Text Secondary:

```css
#94A3B8
```

Border:

```css
#2A3142
```

---

# Tipografia

Font Family:

* Inter
* Geist
* Plus Jakarta Sans

Nunca utilizar fontes decorativas.

---

## Headings

Peso:

```css
700
800
```

---

## Body

Peso:

```css
400
500
```

Tamanho mínimo:

```css
16px
```

---

# Espaçamento

Sistema baseado em escala:

```text
4
8
12
16
24
32
48
64
96
```

Não utilizar valores arbitrários.

---

# Layout

Container principal:

```css
max-width: 1200px;
margin: auto;
```

Grid responsivo.

Sempre respeitar respiro visual.

---

# Estrutura da Aplicação

## Landing Page

Objetivo:

Converter visitantes em jogadores.

Seções:

* Hero
* Benefícios
* Estatísticas
* Ranking
* CTA

---

## Home

Exibir:

* Última pontuação
* Streak atual
* Próximo desafio
* Ranking resumido

A home deve incentivar o usuário a jogar imediatamente.

---

# Tela de Jogo

Tela mais importante do produto.

Recebe máxima prioridade.

---

## Estrutura

### Header

Exibir:

* Logo
* Nível
* XP
* Streak
* Avatar

---

### Progress Section

Exibir:

* Tentativas restantes
* Pontuação da rodada
* Dificuldade

---

### Quote Card

Elemento principal.

Características:

* Grande destaque visual
* Excelente legibilidade
* Alto contraste
* Espaçamento generoso

O trecho deve parecer valioso.

---

### Input de Resposta

Experiência semelhante a busca moderna.

Funcionalidades:

* Autocomplete
* Sugestões
* Navegação por teclado
* Resposta instantânea

---

### Ações

Botão primário:

```text
Responder
```

Botão secundário:

```text
Pedir Dica
```

Botão terciário:

```text
Pular
```

---

# Sistema de Dicas

Ao solicitar uma dica:

* Abrir modal
* Mostrar custo da dica
* Mostrar impacto na pontuação

A geração da dica deve possuir loading elegante.

Utilizar skeleton loaders.

Nunca exibir spinner genérico.

---

# Estados de Loading

Todo carregamento deve possuir:

* Skeleton
* Placeholder
* Transições suaves

Evitar telas vazias.

---

# Feedback de Acerto

Ao acertar:

* Confetti discreto
* Card de vitória
* Pontos ganhos
* Curiosidade gerada pela IA

O usuário deve sentir recompensa imediata.

---

# Feedback de Erro

Ao errar:

* Não utilizar mensagens agressivas
* Não utilizar vermelho excessivo

Exibir:

```text
Não foi dessa vez.
Tente novamente ou solicite uma dica.
```

---

# Ranking

Exibir:

* Global
* Semanal
* Mensal

Destacar Top 3.

Utilizar visual semelhante a jogos competitivos.

---

# Perfil

Exibir:

* Avatar
* Nível
* XP
* Acertos
* Taxa de sucesso
* Livros descobertos
* Conquistas

---

# Sistema de Progressão

Todo usuário possui:

* XP
* Nível
* Streak
* Ranking

Exemplo:

```text
Nível 15
5.240 XP
12 dias consecutivos
```

---

# Sistema de Conquistas

Exemplos:

Primeiro Acerto

```text
Primeira Página
```

10 Livros

```text
Leitor Iniciante
```

50 Livros

```text
Explorador Literário
```

100 Livros

```text
Bibliotecário
```

500 Livros

```text
Guardião dos Clássicos
```

---

# Componentes Obrigatórios

Criar componentes reutilizáveis para:

* Button
* Card
* Modal
* Tooltip
* Badge
* Avatar
* ProgressBar
* Skeleton
* Toast
* RankingCard
* AchievementCard
* QuoteCard

Nenhuma página deve implementar componentes diretamente.

Tudo deve ser reutilizável.

---

# Animações

Somente:

* Fade
* Scale
* Slide

Duração:

```css
150ms
250ms
300ms
```

Evitar animações chamativas.

---

# Acessibilidade

Obrigatório:

* Navegação por teclado
* Contraste AA
* Labels corretas
* Focus visível
* Compatibilidade com leitores de tela

---

# Performance

Metas:

```text
Lighthouse Performance >= 90
Accessibility >= 95
Best Practices >= 95
SEO >= 90
```

---

# Regra Máxima

Sempre que existir dúvida de implementação, perguntar:

"Isso parece um produto que poderia competir no mercado atual?"

Se a resposta for não, revisar a solução.
