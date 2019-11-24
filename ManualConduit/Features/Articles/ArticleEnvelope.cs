using ManualConduit.Domain;

namespace ManualConduit.Features.Articles
{
    public class ArticleEnvelope
    {
        public Article Article { get; }

        public ArticleEnvelope(Article article)
        {
            Article = article;
        }
    }
}
