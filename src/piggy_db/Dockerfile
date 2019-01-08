FROM mysql:5.7.16
MAINTAINER xuanye.wan@gmail.com

ENV MYSQL_DATABASE=PiggyMetrics \
    MYSQL_ROOT_PASSWORD=Welcome@123

ADD schema.sql /docker-entrypoint-initdb.d

EXPOSE 3306
