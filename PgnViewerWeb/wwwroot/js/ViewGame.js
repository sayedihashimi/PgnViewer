var currentMove = {
    moveId: 0
}

var movesPlayed = [];

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

function MoveTo(caller, moveId) {
    // alert('moveto called with: [' + moveId + ' ]');

    if (moveId == currentMove.moveId) {
        return;
    }

    if (moveId > currentMove.moveId) {
        for (var id = currentMove.moveId; id < moveId; id++) {
            // movesToMake.push(window.moves[id].Move);
            var moveResult = window.chess.move(window.moves[id].Move, { verbose: true });
            if (moveResult) {
                window.cg6.move(moveResult.from, moveResult.to);
                MoverookIfCastle(moveResult);
            }
        }
    }

    if (moveId < currentMove.moveId) {
        for (var id = currentMove.moveId; id > moveId; id--) {
            var undoResult = window.chess.undo();
            if (undoResult) {
                window.cg6.move(undoResult.to, undoResult.from);
                UndorookIfCastle(undoResult);
            }
        }
    }

    currentMove.moveId = moveId;
}

function MoverookIfCastle(moveResult) {
    if (moveResult && moveResult.flags) {
        if (moveResult.flags.includes(window.chess.FLAGS.KSIDE_CASTLE)) {
            if (moveResult.color === window.chess.WHITE) {
                // move the white kingside rook
                window.cg6.move('h1','f1');
            }
            else {                
                // move the black kingside rook
                window.cg6.move('h8', 'f8');
            }
        }
        else if (moveResult.flags.includes(window.chess.FLAGS.QSIDE_CASTLE)) {
            if (moveResult.color === window.chess.WHITE) {
                window.cg6.move('a1', 'd1');
            }
            else {
                window.cg6.move('a8', 'd8');
            }
        }
    }
}

function UndorookIfCastle(undoResult) {
    if (undoResult && undoResult.flags) {
        if (undoResult.flags.includes(window.chess.FLAGS.KSIDE_CASTLE)) {
            if (undoResult.color === window.chess.WHITE) {
                // move the white kingside rook
                window.cg6.move('f1', 'h1');
            }
            else {
                // move the black kingside rook
                window.cg6.move('f8', 'h8');
            }
        }
        else if (undoResult.flags.includes(window.chess.FLAGS.QSIDE_CASTLE)) {
            if (undoResult.color === window.chess.WHITE) {
                window.cg6.move('d1', 'a1');
            }
            else {
                window.cg6.move('d8', 'a8');
            }
        }
    }
}

function MoveToOld(caller, id, playerColor) {
    alert('movetoOLD:[' + id + '] color:[' + playerColor + ']')
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
    var nextMoveNum = currentMove.moveId + 1;
    if (nextMoveNum >= window.moves.length - 1) {
        nextMoveNum = window.moves.length - 1;
    }

    MoveTo(null, nextMoveNum);
}

function MovePrevious() {
    var nextMoveNum = currentMove.moveId - 1;
    if (nextMoveNum < 0) {
        nextMoveNum = 0;
    }

    MoveTo(null, nextMoveNum);
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
})();

