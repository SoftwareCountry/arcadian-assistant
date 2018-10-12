import { StyleSheet, Dimensions } from 'react-native';

const screenHeight = Dimensions.get('window').height;

export const employeesListStyles  = StyleSheet.create({
    company: {
        backgroundColor: '#FFF'
    },
    view: {
        paddingTop: 5,
        backgroundColor: '#FFF'
    },
});

export const employeesListItemStyles = StyleSheet.create({
    layout: {
        height: 80,
        flex: 1,
        flexDirection: 'row',
        justifyContent: 'center',
        alignItems: 'center'
    },
    avatarContainer: {
        width: 72,
        height: 72,
        marginLeft: 12,
    },
    avatarOuterFrame: {
        borderWidth: 1
    },
    avatarImage: {
        borderWidth: 2
    },
    info: {
        flex: 1,
        marginRight: 12,
        marginLeft: 12,
        flexDirection: 'column',
        justifyContent: 'center'
    },
    baseText: {
        fontFamily: 'Helvetica-Light',
        fontSize: 9,
        paddingTop: 2
    },
    name: {
        fontFamily: 'Helvetica-Light',
        fontSize: 15,
        paddingBottom: 2
    }
});

export const companyItemStyles = StyleSheet.create({
    layout: {
        flex: 2,
        height: 90,
        overflow: 'hidden',
        borderTopWidth: 1,
        borderColor: 'rgba(0, 0, 0, 0.15)'
    },
    innerLayout: {
        height: 90,
        flex: 1,
        flexDirection: 'row',
        justifyContent: 'center',
        alignItems: 'center'
    },
    avatarContainer: {
        width: 56,
        height: 56,
        marginLeft: 52,
    },
    avatarOuterFrame: {
        borderWidth: 1
    },
    avatarImage: {
        borderWidth: 2
    },
    info: {
        flex: 1,
        height: 56,
        marginRight: 12,
        marginLeft: 12,
        flexDirection: 'column',
        justifyContent: 'flex-start',
        alignItems: 'flex-start'
    },
    baseText: {
        fontFamily: 'Helvetica-Light',
        fontSize: 9,
        paddingTop: 2,
        paddingBottom: 10
    },
    depabbText: {
        fontFamily: 'Helvetica-Light',
        fontSize: 9,
        paddingTop: 2,
        color: '#2FAFCC',

    },
    name: {
        fontFamily: 'Helvetica-Light',
        fontSize: 15,
        paddingBottom: 2
    },
    neighborAvatarContainer: {
        width: 44,
        height: 44,
        position: 'absolute'
    },
    neighborAvatarOuterFrame: {
        borderWidth: 1
    },
    neighborAvatarImage: {
        borderWidth: 2
    }
});

export const companyTinyItemStyles = StyleSheet.create({
    layout: {
        height: 90,
        flex: 0,
        overflow: 'hidden',
        borderTopWidth: 1,
        borderColor: 'rgba(0, 0, 0, 0.15)'
    },
    innerLayout: {
        height: 38,
        flex: 1,
        flexDirection: 'row',
        justifyContent: 'center',
        alignItems: 'center'
    },
    avatarContainer: {
        width: 25,
        height: 25,
        marginLeft: 32,
    },
    avatarOuterFrame: {
        borderWidth: 1
    },
    avatarImage: {
        borderWidth: 1
    },
    info: {
        flex: 1,
        marginRight: 32,
        marginLeft: 12,
        flexDirection: 'row',
        justifyContent: 'flex-start',
        alignItems: 'flex-start'
    },
    baseText: {
        fontFamily: 'Helvetica-Light',
        fontSize: 11,
    },
    name: {
        fontFamily: 'Helvetica-Light',
        fontSize: 11,
    },
    neighborAvatarContainer: {
        width: 44,
        height: 44,
        position: 'absolute'
    }
});

export const companyDepartments = StyleSheet.create({
    levelContainer: {
        backgroundColor: '#fff'
    },
    nodesContainer: {
        borderTopWidth: 1,
        borderColor: 'rgba(0, 0, 0, 0.15)',        
		position: 'relative',
        height: Dimensions.get('window').height * 0.12
    },
    nodesSwipeableContainer: {
        flex: 1,
        flexDirection: 'row',
        position: 'absolute'
    }
});

const contentMargin = screenHeight * 0.018;

export const companyDepartmentsAnimatedNode = StyleSheet.create({
    container: {
        flexDirection: 'row',
    },
    stickyContainer: {
		justifyContent: 'center',
        alignItems: 'center',
        zIndex: 1
    },
    scaleContainer: {
		justifyContent: 'center',
		alignItems: 'center'
    },
    content: {
        marginLeft: contentMargin,
        justifyContent: 'space-between',
        marginTop: contentMargin,
        marginBottom: contentMargin 
    },
    contentPosition: {
        fontSize: 10
    },
    contentDepartmentAbbreviation: {
        fontSize: 9,
        color: '#2FAFCC',        
    }
});

export const companyDepartmentLevelPeople = StyleSheet.create({
    list: {
        height: '100%',
        padding: screenHeight * 0.02
    },
    listItem: {
        flexDirection: 'row', 
        flex: 1,
        height: screenHeight * 0.052,
        marginBottom: 10
    },
    listItemAvator: {
        flex: 1
    },
    listItemContent: {
        flex: 5,
        flexDirection: 'row',
        alignItems: 'center'
    },
    listItemName: {
        fontSize: 11,
        fontWeight: '600'
    },
    listItemPosition: {
        fontSize: 11
    }    
});