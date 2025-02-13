namespace BusinessMotors.Application.Queries
{
    public static class GetVersoesQuery
    {
        public sealed record Versoes(int take, string nome = "", int skip = 0) : IRequest<List<VersaoResponse>>;

        public class Validator : AbstractValidator<Versoes>
        {
            public Validator()
            {
                RuleFor(d => d.take)
                .NotEmpty()
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(100)
                .WithMessage("Take deve ser maior ou igual a 1 e menor ou igual a 100");
            }
        }
    }
}