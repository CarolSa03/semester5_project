import {Router} from 'express';
import {Container} from 'typedi';
import ExecutionController from '../../controllers/ExecutionController';

const route = Router();

export default (app: Router) => {
    app.use('/executions', route);

    const ctrl = Container.get(ExecutionController);

    // POST /api/executions/register
    route.post(
        '/register',
        (req, res, next) => ctrl.registerOperation(req, res, next)
    );
};