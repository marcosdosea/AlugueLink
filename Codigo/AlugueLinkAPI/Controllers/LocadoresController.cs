using AutoMapper;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AlugueLinkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocadoresController : ControllerBase
    {
        private readonly ILocadorService _locadorService;
        private readonly IMapper _mapper;

        public LocadoresController(ILocadorService locadorService, IMapper mapper)
        {
            _locadorService = locadorService;
            _mapper = mapper;
        }

        // GET: api/<LocadoresController>
        [HttpGet]
        public ActionResult Get(int page = 1, int pageSize = 10)
        {
            var listaLocadores = _locadorService.GetAll(page, pageSize);
            if (listaLocadores == null)
                return NotFound();
            
            var locadoresViewModel = _mapper.Map<IEnumerable<LocadorViewModel>>(listaLocadores);
            return Ok(locadoresViewModel);
        }

        // GET api/<LocadoresController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            Locador locador = _locadorService.Get(id);
            if (locador == null)
                return NotFound();
            
            var locadorViewModel = _mapper.Map<LocadorViewModel>(locador);
            return Ok(locadorViewModel);
        }

        // POST api/<LocadoresController>
        [HttpPost]
        public ActionResult Post([FromBody] LocadorViewModel locadorModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var locador = _mapper.Map<Locador>(locadorModel);
            _locadorService.Create(locador);

            return Ok();
        }

        // PUT api/<LocadoresController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] LocadorViewModel locadorModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var locador = _mapper.Map<Locador>(locadorModel);
            if (locador == null)
                return NotFound();

            _locadorService.Edit(locador);

            return Ok();
        }

        // DELETE api/<LocadoresController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            Locador locador = _locadorService.Get(id);
            if (locador == null)
                return NotFound();

            _locadorService.Delete(id);
            return Ok();
        }

        // GET api/<LocadoresController>/search/nome/{nome}
        [HttpGet("search/nome/{nome}")]
        public ActionResult GetByNome(string nome)
        {
            var locadores = _locadorService.GetByNome(nome);
            return Ok(locadores);
        }

        // GET api/<LocadoresController>/search/cpf/{cpf}
        [HttpGet("search/cpf/{cpf}")]
        public ActionResult GetByCpf(string cpf)
        {
            var locadores = _locadorService.GetByCpf(cpf);
            return Ok(locadores);
        }

        // GET api/<LocadoresController>/email/{email}
        [HttpGet("email/{email}")]
        public ActionResult GetByEmail(string email)
        {
            var locador = _locadorService.GetByEmail(email);
            if (locador == null)
                return NotFound();
            
            var locadorViewModel = _mapper.Map<LocadorViewModel>(locador);
            return Ok(locadorViewModel);
        }
    }
}