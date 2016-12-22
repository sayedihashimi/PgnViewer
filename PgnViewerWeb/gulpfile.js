/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');

gulp.task('default', function () {
    gulp.src('./node_modules/chessground/assets/**/*')
        .pipe(gulp.dest('./wwwroot/assets'));

    gulp.src('./node_modules/chessground/examples/**/*')
        .pipe(gulp.dest('./wwwroot/examples'));

    gulp.src('./node_modules/chessground/chessground.js')
        .pipe(gulp.dest('./wwwroot'));
});