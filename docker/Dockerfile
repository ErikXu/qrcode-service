FROM alpine:3.14

RUN sed -i 's/dl-cdn.alpinelinux.org/mirrors.aliyun.com/g' /etc/apk/repositories

# Install .Net dependancies
RUN apk update
RUN apk add --no-cache \
        ca-certificates \
        krb5-libs \
        libgcc \
        libintl \
        libssl1.1 \
        libstdc++ \
        zlib

# Install python3
ENV PYTHONUNBUFFERED=1
RUN apk add --update --no-cache python3 && ln -sf python3 /usr/bin/python
RUN python3 -m ensurepip
RUN pip3 install --no-cache --upgrade pip setuptools


# Install amzq
RUN apk add build-base python3-dev
RUN pip install amzqr

COPY . /app
WORKDIR /app
EXPOSE 5000

ENTRYPOINT ["/app/QRCodeService", "--urls=http://*:5000"]