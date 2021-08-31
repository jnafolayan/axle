FROM node:14.15.5

WORKDIR /app

COPY package.json ./

RUN npm install yarn

RUN npx yarn install

COPY . ./

CMD ["yarn", "start"]
