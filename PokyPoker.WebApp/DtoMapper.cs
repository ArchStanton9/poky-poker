using System.Collections.Generic;
using System.Collections.Immutable;
using AutoMapper;
using PokyPoker.Contracts;
using PokyPoker.Domain;

namespace PokyPoker.WebApp
{
    public class DtoMapper
    {
        public DtoMapper()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<RoundDto, Round>();
                config.CreateMap<ActDto, Act>();
                config.CreateMap<CardDto, Card>();
                config.CreateMap<BettingRulesDto, BettingRules>();
                config.CreateMap<PlayerDto, Player>();
                config.CreateMap<IEnumerable<CardDto>, Hand>()
                    .ConstructUsing(cd => new Hand(Mapper.Map<Card[]>(cd)));
            });
        }

        public GameDto Map(Game game)
        {
            var dto = new GameDto();
            dto.Players = Mapper.Map<List<PlayerDto>>(game.Players);
            dto.Rounds = Mapper.Map<List<RoundDto>>(game.Rounds);
            dto.Table = Mapper.Map<List<CardDto>>(game.Table);
            dto.Rules = Mapper.Map<BettingRulesDto>(game.Rules);

            return dto;
        }

        public Game Map(GameDto gameDto)
        {
            var rules = Mapper.Map<BettingRules>(gameDto.Rules);
            var players = MapImmutableArray<Player, PlayerDto>(gameDto.Players);
            var hand = new Hand(Mapper.Map<Card[]>(gameDto.Table));
            var rounds = MapImmutableArray<Round, RoundDto>(gameDto.Rounds);

            return new Game(rules, players, hand, rounds);
        }

        private static ImmutableArray<TTarget> MapImmutableArray<TTarget, TSource>(IEnumerable<TSource> values)
        {
            var builder = ImmutableArray.CreateBuilder<TTarget>();
            builder.AddRange(Mapper.Map<IEnumerable<TTarget>>(values));
            return builder.ToImmutable();
        }
    }
}
