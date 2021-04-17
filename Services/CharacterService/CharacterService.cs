using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper mapper;
        public CharacterService(IMapper mapper)
        {
            this.mapper = mapper;
        }
        private static List<Character> characters = new List<Character>{
            new Character(),
            new Character{ Id = 1, Name = "Sam"}
        };

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto character)
        {
            Character _character = mapper.Map<Character>(character);
            _character.Id = characters.Max(x => x.Id) + 1;
            characters.Add(_character);
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>
            {
                Data = (characters.Select(x => mapper.Map<GetCharacterDto>(x))).ToList()
            };
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>
            {
                Data = (characters.Select(x => mapper.Map<GetCharacterDto>(x))).ToList()
            };
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>
            {
                Data = mapper.Map<GetCharacterDto>(characters.FirstOrDefault(x => x.Id == id))
            };
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto character)
        {
            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character _character = characters.FirstOrDefault(x => x.Id == character.Id);
                _character.Name = character.Name;
                _character.Class = character.Class;
                _character.Defense = character.Defense;
                _character.HitPoints = character.HitPoints;
                _character.Intelligence = character.Intelligence;
                _character.Strength = character.Strength;

                serviceResponse.Data = mapper.Map<GetCharacterDto>(_character);
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
                Character _character = characters.First(x => x.Id == id);
                characters.Remove(_character);

                serviceResponse.Data = (characters.Select(x => mapper.Map<GetCharacterDto>(x))).ToList();
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