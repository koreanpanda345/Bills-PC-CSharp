using System.Threading.Tasks;
using PokeApiNet;
using PokemonSpecies = PokeAPI.PokemonSpecies;

namespace BillsPC.Pokemon
{
    /// <summary>
    /// This handles PokeApi
    /// </summary>
    public static class Api
    {
        /// <summary>
        /// The PokeApiClient Object
        /// </summary>
        private static PokeApiClient PokeClient { get; set; } = new PokeApiClient();
        /// <summary>
        /// Gets the data for the requested pokemon.
        /// </summary>
        /// <param name="name">The pokemon's name.</param>
        /// <returns>The pokemon's data if it exist as a Pokemon object.</returns>
        /// <exception cref="System.NullException">If the pokemon's data doesn't exist, then this will return null.</exception>
        public static async Task<PokeApiNet.Pokemon> GetPokemon(string name)
        {
            if (PokeClient.GetResourceAsync<PokeApiNet.Pokemon>(name).IsCanceled) return null;
            PokeApiNet.Pokemon pokemon = await PokeClient.GetResourceAsync<PokeApiNet.Pokemon>(name);
            return pokemon;

        }
        /// <summary>
        /// Gets the data for the requested pokemon.
        /// </summary>
        /// <param name="id">the pokemon's national dex id.</param>
        /// <returns>The pokemon's data if it exist as a Pokemon Object.</returns>
        /// <exception cref="System.NullException">If the pokemon's data doesn't exist, then this will return null.</exception>
        public static async Task<PokeApiNet.Pokemon> GetPokemon(int id)
        {
            if (PokeClient.GetResourceAsync<PokeApiNet.Pokemon>(id).IsCanceled) return null;
            PokeApiNet.Pokemon pokemon = await PokeClient.GetResourceAsync<PokeApiNet.Pokemon>(id);

            return pokemon;
        }

        public static async Task<Ability> GetAbility(string name)
        {
            if (PokeClient.GetResourceAsync<Ability>(name).IsCanceled) return null;
            PokeApiNet.Ability ability = await PokeClient.GetResourceAsync<Ability>(name);
            return ability;
        }
    }
}