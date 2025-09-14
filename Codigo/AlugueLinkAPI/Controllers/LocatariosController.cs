using AutoMapper;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AlugueLinkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocatariosController : ControllerBase
    {
        private readonly ILocatarioService _locatarioService;
        private readonly IMapper _mapper;

        public LocatariosController(ILocatarioService locatarioService, IMapper mapper)
        {
            _locatarioService = locatarioService;
            _mapper = mapper;
        }

        // GET: api/<LocatariosController>
        [HttpGet]
        public ActionResult Get(int page = 1, int pageSize = 10)
        {
            var listaLocatarios = _locatarioService.GetAll(page, pageSize);
            if (listaLocatarios == null)
                return NotFound();
            
            var locatariosViewModel = _mapper.Map<IEnumerable<LocatarioViewModel>>(listaLocatarios);
            return Ok(locatariosViewModel);
        }

        // GET api/<LocatariosController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            Locatario locatario = _locatarioService.Get(id);
            if (locatario == null)
                return NotFound();
            
            var locatarioViewModel = _mapper.Map<LocatarioViewModel>(locatario);
            return Ok(locatarioViewModel);
        }

        // POST api/<LocatariosController>
        [HttpPost]
        public ActionResult Post([FromBody] LocatarioViewModel locatarioModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var locatario = _mapper.Map<Locatario>(locatarioModel);
            _locatarioService.Create(locatario);

            return Ok();
        }

        // PUT api/<LocatariosController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] LocatarioViewModel locatarioModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var locatario = _mapper.Map<Locatario>(locatarioModel);
            if (locatario == null)
                return NotFound();

            _locatarioService.Edit(locatario);

            return Ok();
        }

        // DELETE api/<LocatariosController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            Locatario locatario = _locatarioService.Get(id);
            if (locatario == null)
                return NotFound();

            _locatarioService.Delete(id);
            return Ok();
        }

        // GET api/<LocatariosController>/search/nome/{nome}
        [HttpGet("search/nome/{nome}")]
        public ActionResult GetByNome(string nome)
        {
            var locatarios = _locatarioService.GetByNome(nome);
            return Ok(locatarios);
        }

        // GET api/<LocatariosController>/search/cpf/{cpf}
        [HttpGet("search/cpf/{cpf}")]
        public ActionResult GetByCpf(string cpf)
        {
            var locatarios = _locatarioService.GetByCpf(cpf);
            return Ok(locatarios);
        }
    }
}