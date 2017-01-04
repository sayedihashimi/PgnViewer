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
            var moveResult = window.chess.move(window.moves[id].Move, { verbose: true });
            if (moveResult) {
                window.cg6.move(moveResult.from, moveResult.to);
                HandleCastle(moveResult);
                HandlePawnPromotion(moveResult);
                HandleEnPassant(moveResult);
            }
        }
    }

    if (moveId < currentMove.moveId) {
        for (var id = currentMove.moveId; id > moveId; id--) {
            var undoResult = window.chess.undo();
            if (undoResult) {
                window.cg6.move(undoResult.to, undoResult.from);
                HandleUndoCastle(undoResult);
                HandleUndoCapture(undoResult);
                HandleUndoPawnPromotion(undoResult);
                HandleUndoEnPassant(undoResult);
            }
        }
    }

    window.cg6.set({
        turnColor: chessToColor(chess),
        movable: {
            color: chessToColor(chess),
            dests: chessToDests(chess)
        }
    });

    $("#moveList>li>span").removeClass('activeMove');
    $('#move' + moveId).addClass('activeMove');

    var isVisible = 

    currentMove.moveId = moveId;
}

function HandleCastle(moveResult) {
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

function HandlePawnPromotion(moveResult) {
    if (moveResult && moveResult.flags && moveResult.flags.includes(window.chess.FLAGS.PROMOTION)) {
        // place the new piece
        var color = moveResult.color === 'w' ? 'white' : 'black';

        var pieceToAdd = null;
        switch (moveResult.promotion) {
            case window.chess.PAWN:
                pieceToAdd = 'pawn';
                break;
            case window.chess.ROOK:
                pieceToAdd = 'rook';
                break;
            case window.chess.BISHOP:
                pieceToAdd = 'bishop';
                break;
            case window.chess.KNIGHT:
                pieceToAdd = 'knight';
                break;
            case window.chess.QUEEN:
                pieceToAdd = 'queen';
                break;
            case window.chess.KING:
                pieceToAdd = 'king';
                break;
        }

        if (pieceToAdd) {
            var pieceMap = {}
            pieceMap[moveResult.to] = { color: color, role: pieceToAdd };
            window.cg6.setPieces(pieceMap);
        }
    }
}

function HandleEnPassant(moveResult) {
    if (moveResult && moveResult.flags && moveResult.flags.includes(window.chess.FLAGS.EP_CAPTURE)) {
        var pieceToRemove = moveResult.to.charAt(0) + moveResult.from.charAt(1);
        var pieceMap = {};
        pieceMap[pieceToRemove] = null;
        window.cg6.setPieces(pieceMap);
    }
}

function HandleUndoEnPassant(moveResult) {
    if (moveResult && moveResult.flags && moveResult.flags.includes(window.chess.FLAGS.EP_CAPTURE)) {
        var color = moveResult.color === 'w' ? 'white' : 'black';
        var pieceMap = {}
        // after undo move so a piece would have been placed on the to squre that needs to be removed
        pieceMap[moveResult.to] = null;

        var originalSquare = null;
        // place pawn on original square
        if (color === 'white') {
            var origSquareNum = parseInt(moveResult.to.charAt(1)) - 1;
            originalSquare = moveResult.to.charAt(0) + origSquareNum;
            pieceMap[originalSquare] = { role: 'pawn', color: 'black' };
        }
        else {
            var origSquareNum = parseInt(moveResult.to.charAt(1)) + 1;
            originalSquare = moveResult.to.charAt(0) + origSquareNum;
            pieceMap[originalSquare] = { role: 'pawn', color: 'white' };
        }        

        window.cg6.setPieces(pieceMap);
    }
}

function HandleUndoPawnPromotion(moveResult) {
    if (moveResult && moveResult.flags && moveResult.flags.includes(window.chess.FLAGS.PROMOTION)) {
        var color = moveResult.color === 'w' ? 'white' : 'black';
        var pieceMap = {}
        // clear out the queening square
        pieceMap[moveResult.to] = null;
        // place a pawn in the from square
        pieceMap[moveResult.from] = { role: 'pawn', color: color };
        window.cg6.setPieces(pieceMap);
    }
}

function HandleUndoCastle(undoResult) {
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

function HandleUndoCapture(undoResult) {
    if (undoResult && undoResult.captured && undoResult.captured.length > 0) {
        var pieceToAdd = null;
        var color = 'white';
        if (undoResult.color === 'w') {
            color = 'black';
        }

        switch (undoResult.captured) {
            case window.chess.PAWN:
                pieceToAdd = 'pawn';
                break;
            case window.chess.ROOK:
                pieceToAdd = 'rook';
                break;
            case window.chess.BISHOP:
                pieceToAdd = 'bishop';
                break;
            case window.chess.KNIGHT:
                pieceToAdd = 'knight';
                break;
            case window.chess.QUEEN:
                pieceToAdd = 'queen';
                break;
            case window.chess.KING:
                pieceToAdd = 'king';
                break;
        }

        if (pieceToAdd) {
            var pieceMap = {}
            pieceMap[undoResult.to] = { color: color, role: pieceToAdd};
            window.cg6.setPieces(pieceMap);
        }
    }
}

function MoveNext() {
    var nextMoveNum = currentMove.moveId + 1;
    if (nextMoveNum >= window.moves.length) {
        nextMoveNum = window.moves.length;
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

//enquire.register("screen and (max-width:300px)", {
//    match: function () {
//        console.log('match for 500px');
//        $('#ground7').css('height', '250px');
//        $('#ground7').css('width', '250px');
//    }
//});

//enquire.register("screen and (min-width:300px) and (max-width:370px)", {
//    match: function () {
//        console.log('match for 500px');
//        $('#ground7').css('height', '150px');
//        $('#ground7').css('width', '150px');
//        $('#moveList').css('height', '180px').css('font-size', '10pt');
//        $('.moveId').css('display', 'none');
//    },
//    unmatch: function () {
//        $('#moveList').css('font-size', '14pt');
//        $('.moveId').css('display', 'inline');
//    }
//});

//// iPhone 6
//// 375x667
//enquire.register("screen and (min-width:370px) and (max-width:400px)", {
//    match: function () {
//        console.log('match for 400 - 500 px');
//        $('#ground7').css('height', '200px');
//        $('#ground7').css('width', '200px');
//        $('#moveList').css('height', '180px').css('font-size', '10pt');
        
//    }
//});

//// iPhone 6+
//// 414x736
//enquire.register("screen and (min-width:400px) and (max-width:420px)", {
//    match: function () {
//        console.log('match for 400 - 500 px');
//        $('#ground7').css('height', '200px');
//        $('#ground7').css('width', '200px');
//        $('#moveList').css('height', '200px').css('font-size', '10pt');
//    }
//});

//enquire.register("screen and (min-width:420px) and (max-width:500px)", {
//    match: function () {
//        console.log('match for 400 - 500 px');
//        $('#ground7').css('height', '230px');
//        $('#ground7').css('width', '230px');
//        $('#moveList').css('height', '200px').css('font-size', '10pt');
//    }
//});

//enquire.register("screen and (min-width:500px) and (max-width:700px)", {
//    match: function () {
//        console.log('match for 500 - 700 px');
//        $('#ground7').css('height', '300px');
//        $('#ground7').css('width', '300px');
//        $('#moveList').css('height', '280px');
//    }
//});

//enquire.register("screen and (min-width:700px) and (max-width:1000px)", {
//    match: function () {
//        console.log('match for 700 - 1000 px');
//        $('#ground7').css('height', '400px');
//        $('#ground7').css('width', '400px');
//        $('#moveList').css('height', '380px');
//    }
//});
//enquire.register("screen and (min-width:1100px)", {
//    match: function () {
//        console.log('match for 1100 px');
//        $('#ground7').css('height', '700px');
//        $('#ground7').css('width', '700px');
//        $('#moveList').css('height', '680px');
//    }
//});
