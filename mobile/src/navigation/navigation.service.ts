import { NavigationAction, NavigationContainerComponent } from 'react-navigation';

//==============================================================================================
export class NavigationService {
    private instance: NavigationContainerComponent | undefined = undefined;

    //----------------------------------------------------------------------------------------------
    public dispatch(action: NavigationAction) {
        if (this.instance) {
            this.instance.dispatch(action);
        }
    }

    //----------------------------------------------------------------------------------------------
    public setNavigatorRef(ref: NavigationContainerComponent) {
        this.instance = ref;
    }

    //----------------------------------------------------------------------------------------------
    public dispose() {
        this.instance = undefined;
    }
}
