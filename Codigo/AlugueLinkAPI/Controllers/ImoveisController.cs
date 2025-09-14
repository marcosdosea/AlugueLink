using AutoMapper;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AlugueLinkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImoveisController : ControllerBase
    {
        private readonly IImovelService _imovelService;
        private readonly IMapper _mapper;

        public ImoveisController(IImovelService imovelService, IMapper mapper)
        {
            _imovelService = imovelService;
            _mapper = mapper;
        }

        // GET: api/<ImoveisController>
        [HttpGet]
        public ActionResult Get(int page = 1, int pageSize = 10)
        {
            var listaImoveis = _imovelService.GetAll(page, pageSize);
            if (listaImoveis == null)
                return NotFound();
            return Ok(listaImoveis);
        }

        // GET api/<ImoveisController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            Imovel imovel = _imovelService.Get(id);
            if (imovel == null)
                return NotFound("Imóvel não encontrado");
            ImovelViewModel imovelViewModel = _mapper.Map<ImovelViewModel>(imovel);
            return Ok(imovelViewModel);
        }

        // POST api/<ImoveisController>
        [HttpPost]
        public ActionResult Post([FromBody] ImovelViewModel imovelModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var imovel = _mapper.Map<Imovel>(imovelModel);
            _imovelService.Create(imovel);

            return Ok();
        }

        // PUT api/<ImoveisController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] ImovelViewModel imovelModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var imovel = _mapper.Map<Imovel>(imovelModel);
            if (imovel == null)
                return NotFound();

            _imovelService.Edit(imovel);

            return Ok();
        }

        // DELETE api/<ImoveisController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            Imovel imovel = _imovelService.Get(id);
            if (imovel == null)
                return NotFound();

            _imovelService.Delete(id);
            return Ok();
        }
    }
}