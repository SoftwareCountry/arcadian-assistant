import { StyleSheet, Dimensions } from 'react-native';
import Style from '../layout/style';

export const chevronColor = '#2FAFCC';

const circleDiameter = Dimensions.get('window').width * 0.5;
const chevronHeight = 50;
const headerGap = 20; //adds additional gap over the avatar. used mainly for ios
const placeholderHeight = circleDiameter * 0.5; //used to add background for the avatar
const paddingInfoContainer = 12;
const paddingTile = 1;


const avatarContainerZIndex = 2;

export const chevronStyles = StyleSheet.create({
    container: {
        height: chevronHeight
    },
    chevron: {
        width: '100%',
        backgroundColor: 'transparent',
        height: 0,
        zIndex: avatarContainerZIndex,
        borderBottomWidth: chevronHeight,
        borderStyle: 'solid',
        borderLeftColor: 'transparent',
        borderRightColor: 'transparent',
        borderBottomColor: chevronColor,
        position: 'absolute',
        transform: [
            { rotate: '180deg' }
        ]
    }
});

export const profileScreenStyles = StyleSheet.create({
    profileContainer: {
        flex: 1,
        backgroundColor: chevronColor
    },
    employeeDetailsContainer: {
        flex: 1,
        backgroundColor: 'white'
    },
    logoutContainer: {
        marginTop: 5,
        marginBottom: 10
    },
    topHalfView: {
        position: 'absolute',
        backgroundColor: Style.color.base,
        top: 0,
        left: 0,
    },
    bottomHalfView: {
        position: 'absolute',
        backgroundColor: Style.color.white,
        left: 0,
    },
});

export const layoutStyles = StyleSheet.create({
    scrollView: {
        flex: 1,
        backgroundColor: '#fff',
        marginTop: circleDiameter * 0.50
    },
    container: {
        flex: 1,
        alignSelf: 'stretch',
        backgroundColor: '#fff'
    },
    content: {
        flex: 1,
        flexDirection: 'column',
    },
    header: {
        alignSelf: 'center',
        marginBottom: 5,
    },
    avatarContainer: {
        borderRadius: circleDiameter / 2,
        height: circleDiameter,
        width: circleDiameter,
        zIndex: avatarContainerZIndex + 1,
        left: '50%',
        top: -circleDiameter * 0.5 + chevronHeight * 0.5,
        position: 'absolute',
        transform: [{ translateX: circleDiameter * -.5 }],
        alignItems: 'center',
        justifyContent: 'center'
    },
    chevronPlaceholder: {
        height: Math.round(placeholderHeight - chevronHeight * 0.5),
        backgroundColor: chevronColor
    }
});

export const contentStyles = StyleSheet.create({
    name: {
        fontSize: 36,
        textAlign: 'center',
        color: '#000'
    },
    position: {
        fontSize: 13,
        textAlign: 'center',
        color: '#000',
        marginTop: 10
    },
    department: {
        fontSize: 13,
        textAlign: 'center',
        color: '#2FAFCC',
        fontWeight: 'bold',
        marginTop: 8
    },
    infoContainer: {
        flexDirection: 'row',
        padding: paddingInfoContainer,
        marginTop: 20
    },
    contactsContainer: {
        padding: 5,
        paddingBottom: 5,
        alignItems: 'center',
        justifyContent: 'center'
    }
});

export const tileStyles = StyleSheet.create({
    container: {
        width: '25%'
    },
    tile: {
        backgroundColor: 'rgba(47, 175, 204, 0.2)',
        paddingBottom: 8,
        paddingTop: 10,
        flexDirection: 'column',
        height: 66
    },
    iconContainer: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center'
    },
    icon: {
        color: '#18515E',
    },
    text: {
        fontSize: 11,
        textAlign: 'center',
        color: '#18515E',
        paddingTop: 2
    },
    separator: {
        flexBasis: 1,
        backgroundColor: '#fff'
    }
});

export const contactStyles = StyleSheet.create({
    container: {
        flexDirection: 'row',
        height: 60,
        alignItems: 'center'
    },
    iconContainer: {
        width: 50,
        height: 40,
        alignItems: 'center',
        justifyContent: 'center'
    },
    icon: {
        color: '#18515E',
    },
    textContainer: {
        paddingLeft: 20,
        flexDirection: 'column'
    },
    text: {
        fontSize: 14,
        lineHeight: 17,
        color: '#333'
    },
    title: {
        paddingBottom: 3,
        fontSize: 10,
        lineHeight: 12,
        color: 'rgba(0, 0, 0, 0.7)'
    }
});

export const eventStyles = StyleSheet.create({
    container: {
        marginTop: 12,
        padding: 5,
    },
});

