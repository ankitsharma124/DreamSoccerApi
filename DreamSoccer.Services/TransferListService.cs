using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Dtos.TransferList;
using DreamSoccer.Core.Entities;
namespace DreamSoccer.Core.Contracts.Services
{
    public class TransferListService : ITransferListService
    {
        private IMapper _mapper;
        private IPlayerRepository _playerRepository;
        private ITransferListRepository _transferListRepository;
        private IUnitOfWork _unitOfWork;

        public TransferListService(IMapper mapper,
            IPlayerRepository playerRepository,
            ITransferListRepository transferListRepository,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _playerRepository = playerRepository;
            _transferListRepository = transferListRepository;
            _unitOfWork = unitOfWork;
        }

      

        public async Task<IEnumerable<PlayerDto>> SearchPlayerInMarketAsync(SearchPlayerFilter input)
        {
            IQueryable<TransferList> query = await _transferListRepository.SearchPlayerAsync(input);

            var list = new List<Player>();
            return _mapper.Map<List<PlayerDto>>(query.Select(n => n.Player));
        }

       
    }
}