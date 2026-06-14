# BookGuess MVP Roadmap

## Infraestrutura

- [ ] Criar VPS
- [ ] Instalar Docker
- [ ] Configurar Nginx
- [x] Configurar domínio *(nginx.conf criado)*

## Backend

- [x] Criar Solution
- [x] Criar Clean Architecture *(Domain / Application / Infrastructure / Api)*
- [x] JWT *(JwtService + autenticação Bearer)*
- [x] Cadastro *(RegisterUseCase + endpoint POST /api/auth/register)*
- [x] Login *(LoginUseCase + endpoint POST /api/auth/login)*

## Banco

- [x] Users *(entidade + configuração EF + repositório + migration)*
- [x] Books *(entidade + configuração EF + repositório + seed com 8 livros)*
- [x] Quotes *(BookQuote + repositório + seed com trechos)*
- [x] Matches *(Match + Guess + repositórios + endpoints completos)*
- [x] Achievements *(Achievement + UserAchievement + seed + lógica de desbloqueio)*

## IA

- [x] AI Service *(BookGuess.AIService — Minimal API dedicada)*
- [x] Claude Integration *(ClaudeService via Anthropic.SDK — claude-haiku-4-5)*
- [x] Qdrant *(QdrantService — coleção books, upsert e busca semântica)*
- [x] Embeddings *(EmbeddingService via Ollama + fallback)*
- [x] Retrieval *(RagPipeline — retrieve-first, generate-second)*

## Frontend

- [x] Layout Base *(MainLayout + NavBar)*
- [x] Login *(Login.razor + persistência em localStorage)*
- [x] Registro *(Register.razor)*
- [x] Dashboard *(Home.razor — landing para visitantes + dashboard para logados)*
- [x] Match Screen *(Match.razor — trecho, palpite, dicas, pontuação, resultado)*
- [x] Ranking *(Ranking.razor — tabs Global/Semanal/Mensal)*

## Deploy

- [x] Docker Compose *(backend, ai-service, mysql, qdrant, nginx)*
- [ ] Produção *(VPS + deploy)*
- [ ] SSL
