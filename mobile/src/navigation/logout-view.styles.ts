import { StyleSheet } from 'react-native';
import Style from '../layout/style';

//----------------------------------------------------------------------------
export const LogoutStyle = StyleSheet.create({
    container: {
        justifyContent: 'center',
        alignItems: 'center',
        height: 35,
        width: 45,
    },
    image: {
        alignSelf: 'center',
        height: 25,
        width: 25,
        tintColor: Style.color.white,
    }
});
