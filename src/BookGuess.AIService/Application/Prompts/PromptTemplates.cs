namespace BookGuess.AIService.Application.Prompts;

public static class PromptTemplates
{
    public static string GenerateHint(int hintLevel, string title, string author, string quote, string ragContext) => hintLevel switch
    {
        1 => $"""
            Você é um assistente literário especializado em literatura brasileira.
            O livro em questão é "{title}" de {author}. NÃO revele o título nem o autor na dica.
            Com base no trecho e contexto abaixo, gere uma dica MUITO VAGA — apenas gênero, época ou estilo literário.

            Trecho do livro: {quote}
            {(string.IsNullOrWhiteSpace(ragContext) ? "" : $"\nContexto adicional: {ragContext}")}

            Gere apenas a dica em português, sem prefácio. Máximo de 2 frases.
            """,

        2 => $"""
            Você é um assistente literário especializado em literatura brasileira.
            O livro em questão é "{title}" de {author}. NÃO revele o título nem o autor na dica.
            Com base no trecho e contexto abaixo, gere uma dica MODERADA — pode mencionar temas centrais ou características de personagens.

            Trecho do livro: {quote}
            {(string.IsNullOrWhiteSpace(ragContext) ? "" : $"\nContexto adicional: {ragContext}")}

            Gere apenas a dica em português, sem prefácio. Máximo de 2 frases.
            """,

        _ => $"""
            Você é um assistente literário especializado em literatura brasileira.
            O livro em questão é "{title}" de {author}. NÃO revele o título nem o autor na dica.
            Com base no trecho e contexto abaixo, gere uma dica FORTE — pode mencionar personagens ou eventos específicos.

            Trecho do livro: {quote}
            {(string.IsNullOrWhiteSpace(ragContext) ? "" : $"\nContexto adicional: {ragContext}")}

            Gere apenas a dica em português, sem prefácio. Máximo de 2 frases.
            """
    };

    public static string GenerateTrivia(string title, string author, string ragContext) => $"""
        Você é um especialista em literatura brasileira. Gere uma curiosidade interessante e educativa sobre o livro abaixo.
        A curiosidade deve ser envolvente, surpreendente e útil para quem acabou de descobrir a obra.

        Livro: "{title}" de {author}
        {(string.IsNullOrWhiteSpace(ragContext) ? "" : $"\nContexto adicional:\n{ragContext}")}

        Gere apenas a curiosidade, em português, sem prefácio nem saudação. Máximo de 3 frases.
        """;

    public static string ExplainMistake(string context, string incorrectGuess) => $"""
        Você é um assistente literário. O usuário tentou adivinhar o livro com a resposta "{incorrectGuess}", mas errou.
        Com base no contexto abaixo, explique brevemente por que essa resposta está incorreta e dê um contexto educativo.
        Não revele o título correto.

        Contexto: {context}

        Gere apenas a explicação. Máximo de 2 frases.
        """;
}
