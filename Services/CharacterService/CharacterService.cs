using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public CharacterService(IMapper mapper, DataContext context)
        {
            this._mapper = mapper;
            this._context = context;
        }
        private static List<Character> characters = new List<Character>{
            new Character(),
            new Character{ Id = 1, Name = "Sam"}
        };

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto character)
        {
            Character _character = _mapper.Map<Character>(character);
            // _character.Id = characters.Max(x => x.Id) + 1;
            // characters.Add(_character);
            _context.Entry(_character).State = EntityState.Added;
            await _context.SaveChangesAsync();

            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>
            {
                Data = (_context.Characters.Select(x => _mapper.Map<GetCharacterDto>(x))).ToList()
            };
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            // ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>
            // {
            //     Data = (characters.Select(x => _mapper.Map<GetCharacterDto>(x))).ToList()
            // };
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>
            {
                Data = await _context.Characters.Select(x => _mapper.Map<GetCharacterDto>(x)).ToListAsync()
            };
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            // ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>
            // {
            //     Data = _mapper.Map<GetCharacterDto>(characters.FirstOrDefault(x => x.Id == id))
            // };
            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>
            {
                Data = _mapper.Map<GetCharacterDto>(await _context.Characters.FirstOrDefaultAsync(x => x.Id == id))
            };

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto character)
        {
            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character _character = await _context.Characters.FirstOrDefaultAsync(x => x.Id == character.Id);

                _character.Name = character.Name;
                _character.Class = character.Class;
                _character.Defense = character.Defense;
                _character.HitPoints = character.HitPoints;
                _character.Intelligence = character.Intelligence;
                _character.Strength = character.Strength;

                _context.Entry(_character).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetCharacterDto>(_character);
            }
            catch (System.Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                Character _character = await _context.Characters.FirstAsync(x => x.Id == id);
                _context.Entry(_character).State = EntityState.Deleted;
                await _context.SaveChangesAsync();

                serviceResponse.Data = (_context.Characters.Select(x => _mapper.Map<GetCharacterDto>(x))).ToList();
            }
            catch (System.Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
    }
}