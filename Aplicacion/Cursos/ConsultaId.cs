using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Cursos;
using Aplicacion.ManejadorError;
using AutoMapper;
using Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion
{
    public class ConsultaId
    {
        
        public class CursoUnico : IRequest<CursoDto> {
            public Guid Id {get;set;}
        }

        public class Manejador : IRequestHandler<CursoUnico, CursoDto>
        {
            private readonly CursosOnlineContext _context;
            private readonly IMapper _mapper;

            public Manejador(CursosOnlineContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<CursoDto> Handle(CursoUnico request, CancellationToken cancellationToken)
            {
                var curso = await _context.Curso
                    .Include(x => x.ComentarioLista)
                    .Include(x => x.PrecioPromocion)
                    .Include(x => x.InstructoresLink)
                    .ThenInclude(y => y.Instructor)
                    .FirstOrDefaultAsync(a => a.CursoId == request.Id);

                var cursoDto = _mapper.Map<Curso, CursoDto>(curso);

                if(curso == null) {
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {mensaje = "No se encontro el curso"});
                }
                return cursoDto;
            }
        }

    }
}