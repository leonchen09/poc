var gulp = require('gulp');
var plumber = require('gulp-plumber');
var compass = require('gulp-compass');
var config = require('../config').compass;
var handleErrors = require('../util/handleErrors');
var livereload = require('gulp-livereload');

gulp.task('compass', function() {
    return gulp.src(config.src)
        .pipe(plumber({
            errorHandler: handleErrors
        }))
        .pipe(compass(config.settings))
        .pipe(gulp.dest(config.dist))
        .pipe(livereload());
});

