var src = 'src';//源文件路径
var dist = './dist';//输出路径
var path = require('path'); //路径插件

module.exports = {
    html: {
        src: [src + '/*.html',src + '/**/*.html',src+'/**/**/*.html'],
        dist: dist + '/'
    },
    compass: {
        all: src + '/scss/**/*.scss',
        src: src + '/scss/*.scss',
        dist: dist + '/css',
        settings: {
            project: path.join(__dirname, '../'),
            http_path: path.join(__dirname, '../dist/'),
            css: dist + '/css',
            sass: src + '/scss',
            image: path.join(__dirname, '../dist/imgs'),
            font: path.join(__dirname, '../dist/fonts')
        }
    },
    images: {
        src: src + '/imgs/**/*',
        dist: dist + '/imgs',
        settings: {optimizationLevel: 5,
            progressive: true,
            interlaced: true
        }
    },
    //app: {
    //    src: [src + '/js/app/*.js'],
    //    dist: dist + '/js'
    //},
    //controllers: {
    //    src: [src + '/js/controllers/*.js'],
    //    dist: dist + '/js'
    //},
    //directives: {
    //    src: [src + '/js/directives/*.js'],
    //    dist: dist + '/js'
    //},
    //services: {
    //    src: [src + '/js/services/*.js'],
    //    dist: dist + '/js'
    //},
    javascript: {
        src: [src + '/js/*.js',src + '/js/*/*.js',src+'/modules/*/js/*.js'],
        dist: dist + '/js'
    },
    fonts: {
        src: src + '/fonts/**/*',
        dist: dist + '/fonts'
    },
    vendor: {
        src: src + '/vendor/**/*',
        dist: dist + '/vendor'
    }
};