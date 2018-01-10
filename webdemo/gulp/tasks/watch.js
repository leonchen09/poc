var gulp = require('gulp');
var config = require('../config');
var livereload = require('gulp-livereload');


/*监控文件变化*/
gulp.task('watch', function() {
    livereload.listen();
    //var server = livereload();
    //gulp.watch('src/**/*.*', function (file) {
    //    server.changed(file.path);
    //});
    gulp.watch(config.html.src, ['html']);
    gulp.watch(config.compass.all, ['compass']);
    //gulp.watch(config.app.src, ['appJs']);
    //gulp.watch(config.controllers.src, ['controllersJs']);
    //gulp.watch(config.directives.src, ['directivesJs']);
    //gulp.watch(config.services.src, ['servicesJs']);
    gulp.watch(config.javascript.src, ['javascript']);
    gulp.watch(config.images.src, ['images']);
    gulp.watch(config.fonts.src, ['fonts']);
});