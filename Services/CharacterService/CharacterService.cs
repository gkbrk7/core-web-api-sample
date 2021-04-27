using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        private static List<Character> characters = new List<Character>{
            new Character(),
            new Character{ Id = 1, Name = "Sam"}
        };

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto character)
        {
            Character _character = _mapper.Map<Character>(character);
            _character.User = await _context.Users.FirstOrDefaultAsync(x => x.Id == GetUserId());
            // _character.Id = characters.Max(x => x.Id) + 1;
            // characters.Add(_character);
            _context.Entry(_character).State = EntityState.Added;
            await _context.SaveChangesAsync();

            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>
            {
                Data = (_context.Characters.Where(c => c.User.Id == GetUserId()).Select(x => _mapper.Map<GetCharacterDto>(x))).ToList()
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
                Data = await _context.Characters.Where(c => c.User.Id == GetUserId()).Select(x => _mapper.Map<GetCharacterDto>(x)).ToListAsync()
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
                Data = _mapper.Map<GetCharacterDto>(await _context.Characters.FirstOrDefaultAsync(x => x.Id == id && x.User.Id == GetUserId()))
            };

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto character)
        {
            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character _character = await _context.Characters.FirstOrDefaultAsync(x => x.Id == character.Id);

                if (_character.User.Id == GetUserId())
                {
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
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character not found";
                }

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
                Character _character = await _context.Characters.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id && x.User.Id == GetUserId());
                if (_character != null)
                {
                    _context.Entry(_character).State = EntityState.Deleted;
                    await _context.SaveChangesAsync();
                    serviceResponse.Data = (_context.Characters.Where(x => x.User.Id == GetUserId()).Select(x => _mapper.Map<GetCharacterDto>(x))).ToList();
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character not found";
                }

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