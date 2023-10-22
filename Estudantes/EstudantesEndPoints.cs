using ApiCrud.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiCrud.Estudantes
{
    public static class EstudantesEndPoints
    {
        public static void AddRoutesEstudante(this WebApplication app)
        {
            var endPointEstudante = app.MapGroup("estudantes");

            // Para criar se usa o Post
            endPointEstudante.MapPost("", async(AddEstudanteResquest request, 
                AppDbContext context, CancellationToken ct ) =>
            {
                var Exist = await context.Estudantes
                .AnyAsync(estudante => estudante.Name == request.Name, ct);

                if (Exist)
                    return Results.Conflict("Ja existe");

                var newEstudante = new Estudante(request.Name);

               await context.Estudantes.AddAsync(newEstudante, ct);
               await context.SaveChangesAsync(ct);

                var estudanteRetorno = new EstudanteDto(newEstudante.Id, newEstudante.Name);

                return Results.Ok(estudanteRetorno);
            });
            // Retornar Todos os Estudantes Cadastrados
            endPointEstudante.MapGet("", async (AppDbContext context, CancellationToken ct) =>
                {
                    var estudantes = await context.Estudantes
                    .Where(estudante => estudante.Ativo)
                    .Select(estudante => new EstudanteDto(estudante.Id, estudante.Name))
                    .ToListAsync(ct);

                    return estudantes;
                });

            // Atualizar Nome Estudante
            endPointEstudante.MapPut("{id:guid}", async (Guid id, UpdateEstudanteRequest request, AppDbContext context, CancellationToken ct) =>
            {
                var estudante = await context.Estudantes
                .SingleOrDefaultAsync(estudante => estudante.Id == id, ct);

                if (estudante == null)
                    return Results.NotFound();

                estudante.RefreshName(request.Name);

                await context.SaveChangesAsync(ct);
                return Results.Ok(new EstudanteDto(estudante.Id, estudante.Name));
            });

            // Deletar
            endPointEstudante.MapDelete("{id}",
                async (Guid id, AppDbContext context, CancellationToken ct) =>
                {
                    var estudante = await context.Estudantes 
                    .SingleOrDefaultAsync(estudante => estudante.Id == id, ct);

                    if (estudante == null)
                        return Results.NotFound();

                    estudante.Disable();

                    await context.SaveChangesAsync(ct);
                    return Results.Ok();
                });
        }
    }
}
