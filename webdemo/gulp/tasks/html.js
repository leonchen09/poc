var gulp = require('gulp');
var plumber = require('gulp-plumber');
var config = require('../config').html;
var handleErrors = require('../util/handleErrors');
var livereload = require('gulp-livereload');
var htmlmin = require('gulp-htmlmin');

gulp.task('html', function() {
    return gulp.src(config.src)
        .pipe(plumber({
            errorHandler: handleErrors
        }))
        .pipe(gulp.dest(config.dist))
        .pipe(livereload());
});

gulp.task('minhtml', function() {
    return gulp.src(config.src)
        .pipe(plumber({
            errorHandler: handleErrors
        }))
        .pipe(htmlmin({ collapseWhitespace: true }))
        .pipe(gulp.dest(config.dist))
        .pipe(livereload());
});