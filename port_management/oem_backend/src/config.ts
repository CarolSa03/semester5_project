import dotenv from 'dotenv';

// Load .env file
const envFound = dotenv.config();
if (envFound.error) {
    console.warn("Couldn't find .env file");
}

export default {
    port: parseInt(process.env.PORT || '3000', 10),

    databaseURL: process.env.MONGODB_URI || "mongodb://127.0.0.1:27017/oemdb",

    api: {
        prefix: '/oem',
    },
    
    logs: {
        level: process.env.LOG_LEVEL || 'info',
    },
};
