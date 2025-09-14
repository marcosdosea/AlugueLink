using AutoMapper;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AlugueLinkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlugueisController : ControllerBase
    {
        private readonly IAluguelService _aluguelService;
        private readonly IMapper _mapper;

        public AlugueisController(IAluguelService aluguelService, IMapper mapper)
        {
            _aluguelService = aluguelService;
            _mapper = mapper;
        }

        // GET: api/<AlugueisController>
        [HttpGet]
        public ActionResult Get(int page = 1, int pageSize = 10)
        {
            var listaAlugueis = _aluguelService.GetAll(page, pageSize);
            if (listaAlugueis == null)
                return NotFound();
            return Ok(listaAlugueis);
        }

        // GET api/<AlugueisController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            Aluguel aluguel = _aluguelService.Get(id);
            if (aluguel == null)
                return NotFound("Aluguel não encontrado");
            AluguelViewModel aluguelViewModel = _mapper.Map<AluguelViewModel>(aluguel);
            return Ok(aluguelViewModel);
        }

        // POST api/<AlugueisController>
        [HttpPost]
        public ActionResult Post([FromBody] AluguelViewModel aluguelModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var aluguel = _mapper.Map<Aluguel>(aluguelModel);
            _aluguelService.Create(aluguel);

            return Ok();
        }

        // PUT api/<AlugueisController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] AluguelViewModel aluguelModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var aluguel = _mapper.Map<Aluguel>(aluguelModel);
            if (aluguel == null)
                return NotFound();

            _aluguelService.Edit(aluguel);

            return Ok();
        }

        // DELETE api/<AlugueisController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            Aluguel aluguel = _aluguelService.Get(id);
            if (aluguel == null)
                return NotFound();

            _aluguelService.Delete(id);
            return Ok();
        }
    }
}