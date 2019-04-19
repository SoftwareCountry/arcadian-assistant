import { StyleSheet } from 'react-native';
import Style from '../layout/style';
import { grid } from '@haskkor/react-native-pincode/src/design/grid';
import { colors } from '@haskkor/react-native-pincode/dist/src/design/colors';

export const splashScreenStyles = StyleSheet.create({
    container: {
        backgroundColor: Style.color.base,
        flex: 1,
        justifyContent: 'center',
        alignItems: 'stretch'
    },
    imageContainer: {
        width: '50%',
        height: '50%',
        alignSelf: 'center',
        justifyContent: 'center',
        alignItems: 'center',
    },
    greeting: {
        color: Style.color.white,
        fontSize: 32,
        textAlign: 'center',
        paddingLeft: 16,
        paddingRight: 16,
    },
    loginText: {
        color: Style.color.base,
        fontSize: 20,
        textAlign: 'center',
        paddingTop: 5,
        paddingBottom: 5,
    },
    loginButtonContainer: {
        width: '50%',
        alignSelf: 'center',
        marginTop: 140,
        backgroundColor: Style.color.white,
        borderRadius: 4,
        paddingLeft: 32,
        paddingRight: 32,
        paddingBottom: 8,
        paddingTop: 8,
    },
    fingerprintContainer : {
        flex: 1,
    },
});

export const pinCodeStyles = StyleSheet.create({
    container: {
        flex: 1,
        width: '100%',
        backgroundColor: Style.color.white
    },
    stylePinCodeTextTitle: {
        fontFamily: 'CenturyGothic',
        fontSize: 20,
        fontWeight: '200',
        lineHeight: grid.unit * 2.5,
    },
    stylePinCodeTextSubtitle: {
        fontFamily: 'CenturyGothic',
        fontSize: 18,
        fontWeight: '200',
        textAlign: 'center'
    },
    styleLockScreenMainContainer: {
        position: 'absolute',
        flex: 1,
        top: 0,
        left: 0,
        width: '100%',
        height: '100%',
        backgroundColor: colors.background,
        alignItems: 'center',
        justifyContent: 'center',
        paddingTop: 50,
    },
    styleLockScreenViewTextLock: {
        justifyContent: 'center',
        alignItems: 'center',
        paddingLeft: grid.unit * 3,
        paddingRight: grid.unit * 3,
        flex: 3,
    },
    styleLockScreenTitle: {
        fontFamily: 'CenturyGothic',
        fontSize: grid.navIcon,
        color: colors.base,
        opacity: 1,
        fontWeight: '200',
        marginBottom: grid.unit * 2,
        textAlign: 'center',
    },
    styleLockScreenViewCloseButton: {
        alignItems: 'center',
        opacity: 1,
        justifyContent: 'center',
        marginTop: grid.unit * 2
    },
    styleLockScreenButton: {
        backgroundColor: Style.color.base,
        borderRadius: 4,
        paddingLeft: 32,
        paddingRight: 32,
        paddingBottom: 8,
        paddingTop: 8,
    },
    styleLockScreenTextButton: {
        color: colors.white,
        fontSize: 20,
        textAlign: 'center',
        paddingTop: 5,
        paddingBottom: 5,
    },
    styleLockScreenText: {
        fontFamily: 'CenturyGothic',
        fontSize: 14,
        color: colors.base,
        lineHeight: 24,
        textAlign: 'center'
    },
    styleLockScreenViewIcon: {
        width: grid.unit * 4,
        justifyContent: 'center',
        alignItems: 'center',
        height: grid.unit * 4,
        borderRadius: grid.unit * 2,
        opacity: grid.mediumOpacity,
        backgroundColor: colors.alert,
        overflow: 'hidden',
        marginBottom: grid.unit * 2
    },
    styleLockScreenLogoutImage: {
        alignSelf: 'center',
        height: 24,
        width: 24,
        tintColor: colors.base,
        marginTop: 2,
    },
    styleLockScreenLogoutButtonText: {
        fontFamily: 'CenturyGothic',
        fontSize: 13,
        color: colors.base,
        textAlign: 'center',
        fontWeight: '200',
        marginTop: 8,
    },
});
