FROM microsoft/aspnet:1.0.0-beta3

RUN apt-get -qq update && apt-get -qqy --no-install-recommends install \
    git \
    unzip

# Install Node.js and Grunt
RUN curl -sL https://deb.nodesource.com/setup | bash - \
    && apt-get install -qqy nodejs \
    && npm install -g grunt-cli

COPY . /app
WORKDIR /app
RUN ["kpm", "restore"]

EXPOSE 5004
ENTRYPOINT ["k", "kestrel"]
