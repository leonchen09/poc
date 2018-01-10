var gulp = require('gulp');
var plumber = require('gulp-plumber');
var javascript = require('../config').javascript;
//var configApp = require('../config').app;
//var configCtrl = require('../config').controllers;
//var configDirectives = require('../config').directives;
//var configServices = require('../config').services;
var jshint = require('gulp-jshint');
var uglify = require('gulp-uglify');
var concat = require('gulp-concat');
var rename = require('gulp-rename');
var handleErrors = require('../util/handleErrors');
var livereload = require('gulp-livereload');

//js
gulp.task('javascript', function() {
    return gulp.src(javascript.src)
        .pipe(plumber({
            errorHandler: handleErrors
        }))
        .pipe(concat('app.js'))
        .pipe(jshint())
        .pipe(jshint.reporter('default'))
        .pipe(rename({
            suffix: '.min'
        }))
        .pipe(gulp.dest(javascript.dist))
        .pipe(livereload());
});

////app
//gulp.task('appJs', function() {
//    return gulp.src(configApp.src)
//        .pipe(plumber({
//            errorHandler: handleErrors
//        }))
//        .pipe(concat('app.js'))
//        .pipe(jshint())
//        .pipe(jshint.reporter('default'))
//        .pipe(rename({
//            suffix: '.min'
//        }))
//        .pipe(gulp.dest(configApp.dist))
//        .pipe(livereload());
//});
//
//gulp.task('minAppJs', function() {
//    return gulp.src(configApp.src)
//        .pipe(plumber({
//            errorHandler: handleErrors
//        }))
//        .pipe(jshint())
//        .pipe(jshint.reporter('default'))
//        .pipe(concat('app.js'))
//        .pipe(rename({
//            suffix: '.min'
//        }))
//        .pipe(uglify())
//        .pipe(gulp.dest(configApp.dist))
//        .pipe(livereload());
//});
//
////ctrl
//gulp.task('controllersJs', function() {
//    return gulp.src(configCtrl.src)
//        .pipe(plumber({
//            errorHandler: handleErrors
//        }))
//        .pipe(concat('controllers.js'))
//        .pipe(jshint())
//        .pipe(jshint.reporter('default'))
//        .pipe(rename({
//            suffix: '.min'
//        }))
//        .pipe(gulp.dest(configCtrl.dist))
//        .pipe(livereload());
//});
//
//gulp.task('minCtrlJs', function() {
//    return gulp.src(configCtrl.src)
//        .pipe(plumber({
//            errorHandler: handleErrors
//        }))
//        .pipe(jshint())
//        .pipe(jshint.reporter('default'))
//        .pipe(concat('controllers.js'))
//        .pipe(rename({
//            suffix: '.min'
//        }))
//        .pipe(uglify())
//        .pipe(gulp.dest(configCtrl.dist))
//        .pipe(livereload());
//});
//
////directive
//gulp.task('directivesJs', function() {
//    return gulp.src(configDirectives.src)
//        .pipe(plumber({
//            errorHandler: handleErrors
//        }))
//        .pipe(concat('directives.js'))
//        .pipe(jshint())
//        .pipe(jshint.reporter('default'))
//        .pipe(rename({
//            suffix: '.min'
//        }))
//        .pipe(gulp.dest(configDirectives.dist))
//        .pipe(livereload());
//});
//
//gulp.task('minDirectiveJs', function() {
//    return gulp.src(configDirectives.src)
//        .pipe(plumber({
//            errorHandler: handleErrors
//        }))
//        .pipe(jshint())
//        .pipe(jshint.reporter('default'))
//        .pipe(concat('directives.js'))
//        .pipe(rename({
//            suffix: '.min'
//        }))
//        .pipe(uglify())
//        .pipe(gulp.dest(configDirectives.dist))
//        .pipe(livereload());
//});
//
////services
//gulp.task('servicesJs', function() {
//    return gulp.src(configServices.src)
//        .pipe(plumber({
//            errorHandler: handleErrors
//        }))
//        .pipe(concat('services.js'))
//        .pipe(jshint())
//        .pipe(jshint.reporter('default'))
//        .pipe(rename({
//            suffix: '.min'
//        }))
//        .pipe(gulp.dest(configServices.dist))
//        .pipe(livereload());
//});
//
//gulp.task('minServicesJs', function() {
//    return gulp.src(configServices.src)
//        .pipe(plumber({
//            errorHandler: handleErrors
//        }))
//        .pipe(jshint())
//        .pipe(jshint.reporter('default'))
//        .pipe(concat('services.js'))
//        .pipe(rename({
//            suffix: '.min'
//        }))
//        .pipe(uglify())
//        .pipe(gulp.dest(configServices.dist))
//        .pipe(livereload());
//});