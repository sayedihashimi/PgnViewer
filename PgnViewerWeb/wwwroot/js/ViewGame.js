
function chessToDests(chess) {
    var dests = {};
    chess.SQUARES.forEach(function (s) {
        var ms = chess.moves({ square: s, verbose: true });
        if (ms.length) dests[s] = ms.map(function (m) { return m.to; });
    });
    return dests;
}

function chessToColor(chess) {
    return (chess.turn() == "w") ? "white" : "black";
}

function MakeNextMove() {

}

function MoveTo(caller, id, playerColor) {
    // alert('moveto:[' + id + '] color:[' + playerColor + ']')
    // go back to the beginning of the game and play moves until we get to the right move

    while (($uresult = window.chess.undo()) != null) {
        console.log('undo move called');
    }

    window.cg6.set({ fen: chess.fen() });

    if (id <= 0) {
        return;
    }

    var moves = window.moves;
    // get the moves
    for (var i = 0; i < moves.length; i++) {
        var cMove = moves[i];
        var cId = cMove.id;
        var cPlayer = cMove.playerColor;

        // if a move exists for white play it
        var whiteMove = cMove.whiteMove
        if (cMove.White) {
            var movePlayed = window.chess.move(cMove.White, { verbose: true });
            if (movePlayed) {
                movesPlayed.push(movePlayed.from + "-" + movePlayed.to);
                window.cg6.move(movePlayed.from, movePlayed.to);
            }
        }

        // play the black move unless this is the last move and move selected was for white

        if (id === (i + 1) && playerColor === 'w') {
            currentMove = id;
            currentColor = 'w';

            ground.set({
                turnColor: chessToColor(chess),
                movable: {
                    color: chessToColor(chess),
                    dests: chessToDests(chess)
                }
            });
            break;
        }

        if (cMove.Black) {
            var movePlayed = window.chess.move(cMove.Black, { verbose: true });
            if (movePlayed) {
                movesPlayed.push(movePlayed.from + "-" + movePlayed.to);
                window.cg6.move(movePlayed.from, movePlayed.to);
            }
        }

        // stop the loop to prevent extra moves played
        if (id === (i + 1)) {
            currentMove = id;
            currentColor = 'b';

            ground.set({
                turnColor: chessToColor(chess),
                movable: {
                    color: chessToColor(chess),
                    dests: chessToDests(chess)
                }
            });
            break;
        }
    }
}

function MoveNext() {
    if (currentColor === 'w') {
        currentColor = 'b';
    }
    else {
        currentMove++;
        currentColor = 'w';
    }

    if (currentMove > window.moves.length || currentMove < 1) {
        currentMove = 1;
        currentColor = 'w'
    }

    MoveTo(null, currentMove, currentColor);
}
function MovePrevious() {
    if (currentColor === 'b') {
        currentColor = 'w';
    }
    else {
        currentMove--;
        currentColor = 'b';
    }

    if (currentMove > window.moves.length || currentMove < 1) {
        currentMove = 0;
        currentColor = 'b'
    }
    MoveTo(null, currentMove, currentColor);
}

(function () {
    document.onkeydown = function (e) {
        if (e.keyCode === 37) {
            // arrow left
            MovePrevious();
        }
        else if (e.keyCode === 39) {
            // arrow right
            MoveNext();
        }
    };

})();

var currentMove = {
    moveId: 1,
    playerColor: 'w'
}

var movesPlayed = [];
var currentMove = 0;
var currentColor = 'b';

(function () {
    var ground;
    var fen = document.getElementById('maingame').getAttribute('data-fen')
    if (!fen) { fen = null; }

    var chess = new Chess();
    if (fen != null && fen.length > 0) {
        chess = new Chess(fen);
    }

    var onMove = function (orig, dest) {
        var moveString = orig + "-" + dest
        var movesPlayedIndex = movesPlayed.indexOf(moveString);
        if (movesPlayedIndex >= 0) {
            // the move was played programatically so don't call .move here. just remove the string from movesPlayed
            movesPlayed.splice(movesPlayedIndex, 1);
        }
        else {
            chess.move({ from: orig, to: dest });
        }

        ground.set({
            turnColor: chessToColor(chess),
            movable: {
                color: chessToColor(chess),
                dests: chessToDests(chess)
            }
        });

        console.log(ground.getFen());
    };

    ground = Chessground(document.getElementById('ground7'), {
        viewOnly: false,
        fen: fen,
        turnColor: chessToColor(chess),
        animation: {
            duration: 500
        },
        movable: {
            free: false,
            color: chessToColor(chess),
            dests: chessToDests(chess),
            showDests: true,
            events: {
                after: onMove
            }
        },
        disableContextMenu: false,
        highlight: {
            lastMove: true
        },
        premovable: {
            enabled: false
        },
        drawable: {
            enabled: true         // enable SVG circle and arrow drawing on the board
        }
    });

    window.chess = chess;
    window.cg6 = ground;

    window.moves = JSON.parse(document.getElementById('maingame').getAttribute('data-moves'));

    currentMove = {
        moveId: 1,
        playerColor: window.chess.turn()
    }
})();

