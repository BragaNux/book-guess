using BookGuess.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookGuess.Infrastructure.Persistence;

public class DataSeeder(BookGuessDbContext context, ILogger<DataSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await context.Database.MigrateAsync(cancellationToken);

        await SeedAchievementsAsync(cancellationToken);
        await SeedBooksAsync(cancellationToken);
        await SeedDailyChallengeAsync(cancellationToken);

        logger.LogInformation("Database seeding completed.");
    }

    private async Task SeedDailyChallengeAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var quotes = await context.BookQuotes
            .Include(q => q.Book)
            .OrderBy(q => q.Book.Difficulty)
            .ThenBy(q => q.Book.Title)
            .ThenBy(q => q.Id)
            .ToListAsync(cancellationToken);

        if (quotes.Count == 0) return;

        var existingDates = await context.DailyChallenges
            .Select(dc => dc.ChallengeDate)
            .ToHashSetAsync(cancellationToken);

        // Quotes já alocadas para a janela dos próximos 31 dias (evitar repetição)
        var usedInWindow = await context.DailyChallenges
            .Where(dc => dc.ChallengeDate >= today && dc.ChallengeDate <= today.AddDays(30))
            .Select(dc => dc.BookQuoteId)
            .ToHashSetAsync(cancellationToken);

        var available = quotes.Where(q => !usedInWindow.Contains(q.Id)).ToList();
        int idx    = 0;
        int seeded = 0;

        for (int day = 0; day < 31; day++)
        {
            var date = today.AddDays(day);
            if (existingDates.Contains(date)) continue;
            if (idx >= available.Count) break;

            var quote     = available[idx++];
            var challenge = DailyChallenge.Create(quote.Id, date, quote.Difficulty);
            await context.DailyChallenges.AddAsync(challenge, cancellationToken);
            seeded++;
        }

        if (seeded > 0)
        {
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded {Count} daily challenges starting from {Date}.", seeded, today);
        }
    }

    private async Task SeedAchievementsAsync(CancellationToken cancellationToken)
    {
        if (await context.Achievements.AnyAsync(cancellationToken)) return;

        var achievements = new[]
        {
            Achievement.Create("FIRST_WIN", "Primeira Página", "Acertou seu primeiro livro.", 50, "🏅"),
            Achievement.Create("FIRST_HINT", "Preciso de Ajuda", "Usou uma dica pela primeira vez.", 10, "💡"),
            Achievement.Create("TEN_BOOKS", "Leitor Iniciante", "Descobriu 10 livros.", 100, "📚"),
            Achievement.Create("FIFTY_BOOKS", "Explorador Literário", "Descobriu 50 livros.", 250, "🔍"),
            Achievement.Create("HUNDRED_BOOKS", "Bibliotecário", "Descobriu 100 livros.", 500, "🏛️"),
            Achievement.Create("FIVE_HUNDRED_BOOKS", "Guardião dos Clássicos", "Descobriu 500 livros.", 1000, "👑"),
            Achievement.Create("SEVEN_DAY_STREAK", "Consistência", "Jogou 7 dias consecutivos.", 150, "🔥"),
        };

        await context.Achievements.AddRangeAsync(achievements, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} achievements.", achievements.Length);
    }

    private async Task SeedBooksAsync(CancellationToken cancellationToken)
    {
        var existingTitles = await context.Books
            .Select(b => b.Title)
            .ToHashSetAsync(cancellationToken);

        var books = new List<(Book book, string[] quotes, string[] trivias)>
        {
            (
                Book.Create("Dom Casmurro", "Machado de Assis", 3, "Romance", "Narrado por Bentinho, o livro gira em torno de sua obsessão por descobrir se Capitu o traiu.", 1899),
                ["Capitu tinha olhos de ressaca, olhos que puxavam, que absorviam, que comiam.", "Era o tempo em que eu era seminarista, isto é, estudante de preparatórios para o Seminário.", "O dr. João da Costa era um homem pequeno, gordo, alegre e bom."],
                ["Dom Casmurro é considerado o maior romance da literatura brasileira.", "A questão 'Capitu traiu ou não traiu?' é um dos maiores debates literários do Brasil.", "Machado de Assis escreveu o livro já sofrendo de epilepsia e em pleno sucesso literário."]
            ),
            (
                Book.Create("O Cortiço", "Aluísio Azevedo", 4, "Naturalismo", "Romance naturalista que retrata a vida em um cortiço no Rio de Janeiro do século XIX.", 1890),
                ["O cortiço acordou, abrindo, não os olhos, mas a sua infinidade de portas e janelas alinhadas.", "Miranda contemplava a sua prosperidade com a serenidade de um homem que sabe bem o que quer.", "Eram cinco horas da manhã e o cortiço já fervia."],
                ["O Cortiço foi inspirado no cortiço Cabeça de Porco, que existiu no Rio de Janeiro.", "Aluísio Azevedo foi considerado o principal representante do naturalismo no Brasil.", "O livro retrata com crueza a mestiçagem e as condições de vida do povo brasileiro."]
            ),
            (
                Book.Create("Iracema", "José de Alencar", 2, "Romantismo", "Lenda do Ceará que conta a história da índia Iracema e o guerreiro branco Martim.", 1865),
                ["Iracema, a virgem dos lábios de mel, que tinha os cabelos mais negros que a asa da graúna.", "Além, muito além daquela serra, que ainda azula no horizonte, nasceu Iracema.", "Martim dormia tranquilo sobre a relva, quando acordou ao som de passos leves."],
                ["Iracema é um anagrama de América, segundo interpretação de muitos críticos.", "José de Alencar quis criar com Iracema um mito fundador para o povo brasileiro.", "O nome Iracema vem do tupi e significa 'lábios de mel'."]
            ),
            (
                Book.Create("Vidas Secas", "Graciliano Ramos", 4, "Modernismo", "Romance que retrata a vida de uma família de retirantes nordestinos fugindo da seca.", 1938),
                ["A catinga ficara para trás, na poeira e no sol, com as ossadas e os grilos.", "Fabiano pensou nisso, achou que era bom. Se não achasse bom, não saberia fazer outra coisa.", "A cachorra Baleia cochilava, sonhando com preás gordos."],
                ["Graciliano Ramos escreveu o livro enquanto estava preso pelo governo Vargas.", "Cada capítulo de Vidas Secas pode ser lido de forma independente, como contos.", "Baleia, a cachorra, é um dos personagens mais memoráveis da literatura brasileira."]
            ),
            (
                Book.Create("Memórias Póstumas de Brás Cubas", "Machado de Assis", 5, "Realismo", "Narrado por um defunto autor, o livro revolucionou a literatura brasileira com sua ironia e pessimismo.", 1881),
                ["Ao verme que primeiro roeu as frias carnes do meu cadáver dedico como saudosa lembrança estas Memórias Póstumas.", "Não tive filhos, não transmiti a nenhuma criatura o legado da nossa miséria.", "Considerem que o estilo é o homem."],
                ["Memórias Póstumas foi o primeiro romance realista brasileiro.", "O protagonista, Brás Cubas, é narrador não confiável, um recurso inovador para a época.", "Machado de Assis foi o primeiro presidente da Academia Brasileira de Letras."]
            ),
            (
                Book.Create("O Guarani", "José de Alencar", 2, "Romantismo", "Romance histórico que narra a história do índio Peri e sua devoção à branca Ceci.", 1857),
                ["Peri, o índio, tinha a força de um touro e a agilidade de uma onça.", "Ceci era loura como o sol, branca como a flor do algodão.", "Dom Antônio de Mariz era um fidalgo português, velho servidor da Coroa."],
                ["O Guarani foi publicado originalmente em folhetim no Jornal do Commercio.", "A obra foi adaptada para ópera pelo compositor Carlos Gomes em 1870.", "José de Alencar criou com Peri o arquétipo do índio heroico da literatura brasileira."]
            ),
            (
                Book.Create("Capitães da Areia", "Jorge Amado", 3, "Modernismo", "Romance que retrata a vida de um grupo de meninos de rua em Salvador nos anos 1930.", 1937),
                ["Pedro Bala era o chefe. Tinha catorze anos e era reconhecido por todos os meninos.", "No trapiche abandonado, os Capitães da Areia dormiam ao relento.", "A cidade de Salvador, vista de longe, era linda. Vista de perto, era outra coisa."],
                ["Capitães da Areia foi queimado em praça pública pelo governo Vargas em 1937.", "Jorge Amado escreveu o romance com apenas 24 anos.", "O livro foi adaptado para o cinema pelo diretor Cecil Thiré em 1989."]
            ),
            (
                Book.Create("Macunaíma", "Mário de Andrade", 5, "Modernismo", "Rapsódia modernista sobre o herói brasileiro sem nenhum caráter, sua jornada e aventuras.", 1928),
                ["Macunaíma, o herói de nossa gente, nasceu no fundo do mato virgem.", "Ai! que preguiça!... disse Macunaíma, bocejando.", "Macunaíma era de cor preta e carecia de muita saúde."],
                ["Mário de Andrade escreveu Macunaíma em apenas seis dias.", "O subtítulo do livro é 'O herói sem nenhum caráter', referindo-se à identidade nacional brasileira.", "O livro mistura mitos indígenas, africanos e europeus para criar uma identidade nacional."]
            ),
            (
                Book.Create("Senhora", "José de Alencar", 2, "Romantismo", "Romance que explora a venda de amor e casamento por conveniência na sociedade carioca do século XIX.", 1875),
                ["Aurélia era bela, e já muito se falava de sua beleza em toda a cidade do Rio de Janeiro.", "O casamento é um negócio como outro qualquer, onde o amor pode ser comprado e vendido.", "Fernando Seixas era um rapaz de boa família, que tinha gastado toda a herança em futilidades."],
                ["Senhora foi o último dos romances urbanos de José de Alencar.", "Aurélia Camargo é considerada um dos primeiros personagens femininos independentes da literatura brasileira.", "A obra critica abertamente o casamento como transação comercial na sociedade do Segundo Reinado."]
            ),
            (
                Book.Create("São Bernardo", "Graciliano Ramos", 4, "Modernismo", "Narrado em primeira pessoa por Paulo Honório, um fazendeiro brutal que reflete sobre a ruína de seu casamento.", 1934),
                ["Vou contar a história de São Bernardo. Narro os fatos como me ocorrem, sem cuidar do estilo.", "Paulo Honório era duro, ambicioso, e não se envergonhava disso.", "Madalena era diferente de tudo que eu havia conhecido. Era delicada e suave."],
                ["São Bernardo foi escrito por Graciliano Ramos em apenas quarenta dias.", "O romance é considerado um dos melhores exemplos da prosa modernista brasileira.", "Paulo Honório é visto como um símbolo da desumanização causada pelo capitalismo agrário."]
            ),
            (
                Book.Create("Quincas Borba", "Machado de Assis", 5, "Realismo", "Romance que satiriza o positivismo e o humanitismo através da trajetória do ingênuo Rubião.", 1891),
                ["Ao vencedor, as batatas. Essa era a filosofia de Quincas Borba.", "Rubião olhava para Sofia com a admiração de quem nunca tinha visto uma mulher tão bela.", "Humanitas! A substância universal, o princípio de todas as coisas."],
                ["Quincas Borba surgiu primeiro como personagem de Memórias Póstumas de Brás Cubas.", "O humanitismo é uma paródia do positivismo de Auguste Comte.", "Machado de Assis usou o romance para criticar a elite brasileira do Segundo Reinado."]
            ),
        };

        int added = 0;
        foreach (var (book, quotes, trivias) in books)
        {
            if (existingTitles.Contains(book.Title)) continue;

            await context.Books.AddAsync(book, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            added++;

            foreach (var quote in quotes)
            {
                var bookQuote = BookQuote.Create(book.Id, quote, book.Difficulty);
                await context.BookQuotes.AddAsync(bookQuote, cancellationToken);
            }

            foreach (var triviaContent in trivias)
            {
                var trivia = Trivia.Create(book.Id, triviaContent, "Curiosidade");
                await context.Trivias.AddAsync(trivia, cancellationToken);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        if (added > 0)
            logger.LogInformation("Seeded {Count} new books.", added);
    }
}
