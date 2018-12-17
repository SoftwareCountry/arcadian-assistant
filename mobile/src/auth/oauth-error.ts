export class OauthError extends Error {
    public name = 'OauthError';

    constructor(public message: string = '') {
        super(message);
        this.message = message;
    }
}
