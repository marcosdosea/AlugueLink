using AutoMapper;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AlugueLinkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagamentosController : ControllerBase
    {
        private readonly IPagamentoService _pagamentoService;
        private readonly IMapper _mapper;

        public PagamentosController(IPagamentoService pagamentoService, IMapper mapper)
        {
            _pagamentoService = pagamentoService;
            _mapper = mapper;
        }

        // GET: api/<PagamentosController>
        [HttpGet]
        public ActionResult Get(int page = 1, int pageSize = 10)
        {
            var listaPagamentos = _pagamentoService.GetAll(page, pageSize);
            if (listaPagamentos == null)
                return NotFound();
            return Ok(listaPagamentos);
        }

        // GET api/<PagamentosController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            Pagamento pagamento = _pagamentoService.Get(id);
            if (pagamento == null)
                return NotFound("Pagamento não encontrado");
            PagamentoViewModel pagamentoViewModel = _mapper.Map<PagamentoViewModel>(pagamento);
            return Ok(pagamentoViewModel);
        }

        // POST api/<PagamentosController>
        [HttpPost]
        public ActionResult Post([FromBody] PagamentoViewModel pagamentoModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var pagamento = _mapper.Map<Pagamento>(pagamentoModel);
            _pagamentoService.Create(pagamento);

            return Ok();
        }

        // PUT api/<PagamentosController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] PagamentoViewModel pagamentoModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var pagamento = _mapper.Map<Pagamento>(pagamentoModel);
            if (pagamento == null)
                return NotFound();

            _pagamentoService.Edit(pagamento);

            return Ok();
        }

        // DELETE api/<PagamentosController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            Pagamento pagamento = _pagamentoService.Get(id);
            if (pagamento == null)
                return NotFound();

            _pagamentoService.Delete(id);
            return Ok();
        }
    }
}