import { Linking } from "react-native";
import { ajaxGet, ajaxPost } from "rxjs/observable/dom/AjaxObservable";

export class LogoutRequest {
    constructor(private readonly logoutUrl: string) {
    }

    public perform() {
        return Linking.openURL(this.logoutUrl);
         //ajaxGet(this.logoutUrl).toPromise()
    }
}