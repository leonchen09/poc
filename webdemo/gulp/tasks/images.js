var gulp = require('gulp');
var plumber = require('gulp-plumber');
var config = require('../config').images;
var imagemin = require('gulp-imagemin');
var handleErrors = require('../util/handleErrors');

//不压缩
gulp.task('images', function() {
    return gulp.src(config.src)
        .pipe(plumber({
            errorHandler: handleErrors
        }))
        .pipe(gulp.dest(config.dist));
});

//压缩
gulp.task('minimages', function() {
    return gulp.src(config.src)
        .pipe(plumber({
            errorHandler: handleErrors
        }))
        .pipe(imagemin(config.settings))
        .pipe(gulp.dest(config.dist));
});