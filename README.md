#GamestatsBase

This is a library to assist with implementing custom HTTP Gamestats servers as
used by some games formerly hosted by the GameSpy network.

Since my experience with the protocol is limited to Nintendo DS games, there
are bound to be some design assumptions I've made along the way which may not
always hold. If this library doesn't support a particular game's usage, please
send a pull request or open an issue. Please send packet captures with any pull
requests or new issues so I can verify your fix.

Since I have not yet settled on a license, please ask before using this in your
project. I can be contacted at #altwfc in Rizon.

##Usage

In general, you'll want to perform URL rewriting in your global.asax handler to
forward requests to the .ashx file where you implement your gamestats logic. In
the .ashx handler, instead of inheriting IHttpHandler, you will inherit from
GamestatsBase.GamestatsHandler and add a constructor where you provide the
game-specific secret constants. (These constants will be contained within the
game binary and helping you find them is outside the scope of this project.)

Please see the included (albeit incomplete) Tetris DS example and the more
robust [Pokémon Foundations](https://github.com/mm201/pkmnFoundations) project
for examples of this library in use.
