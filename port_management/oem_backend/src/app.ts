import 'reflect-metadata'; 
import express from 'express';
import mongoose from 'mongoose';
import config from './config';
import loaders from './loaders'; 
import cors from 'cors';
import { isCelebrateError } from 'celebrate';

import operationPlanRoute from './api/routes/operationPlanRoute';
import executionRoute from './api/routes/executionRoute';
import incidentRoute from './api/routes/incidentRoute';
import incidentTypeRoute from './api/routes/incidentTypeRoute';
import vesselVisitExecutionRoute from './api/routes/vesselVisitExecutionRoute';

async function startServer() {
    const app = express();

    // 1. Enable CORS (Critical for Frontend connection)
    app.use(cors());

    // 2. Connect to Database
    try {
        await mongoose.connect(config.databaseURL);
        console.log('DB Loaded and connected!');
    } catch (err) {
        console.error('DB Connection Failed:', err);
        process.exit(1);
    }

    // 3. Load Dependency Injection & other loaders
    await loaders();

    // 4. Configure Express Parsing
    app.use(express.json());

    // 5. Configure Routes
    const apiRouter = express.Router();

    // Attach sub-routes to the router
    executionRoute(apiRouter);
    incidentRoute(apiRouter);
    incidentTypeRoute(apiRouter); 
    operationPlanRoute(apiRouter);
    vesselVisitExecutionRoute(apiRouter);

    app.use(config.api.prefix, apiRouter);

    // --- Error handling for validation (celebrate) and generic errors ---
    // This must be registered AFTER all routes
    app.use((err: any, req: any, res: any, next: any) => {
        // Celebrate validation errors -> 400
        if (isCelebrateError(err) || (err && err.details && err.details.get)) {
            return res.status(400).json({ error: 'Validation failed' });
        }

        // Joi standalone errors (array of details)
        if (err && err.details && Array.isArray(err.details)) {
            return res.status(400).json({ error: 'Validation failed' });
        }

        // Fallback generic error
        console.error('🔥 Unknown server error:', err);
        return res.status(500).json({ error: err?.message || 'Internal Server Error' });
    });

    // 6. Start Server
    app.listen(config.port, () => {
        console.log(`
      ################################################
      🛡️  OEM Server listening on port: ${config.port} 🛡️ 
      ################################################
      Access at: http://localhost:${config.port}${config.api.prefix}
    `);
    });
}

export { startServer };
