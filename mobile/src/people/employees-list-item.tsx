import React from 'react';
import { TouchableHighlight, StyleSheet, Platform, Text, View, Image } from 'react-native';
import { Avatar } from '../people/avatar';
import { Employee } from '../reducers/organization/employee.model';
import { LayoutEvent } from 'react-navigation';

interface EmployeesListItemProps {
    employee: Employee;
    id: string;
}

const styles = StyleSheet.create({
    layout: {
        flex: 1,
        flexDirection: 'row',
        justifyContent: 'center',
        paddingTop: 0,
        paddingBottom: 5
    },
    imgContainer: {
        marginTop: 5,
        flex: 2
    },
    img: {
        flex: 1
    },
    info: {
        flex: 6,
        flexDirection: 'column',
        alignSelf: 'flex-start',
        paddingLeft: 13
    },
    title: {
        fontSize: 19,
        textAlign: 'left',
        fontWeight: '400',
        letterSpacing: 2,
    },
    text: {
        fontSize: 15,
        textAlign: 'left',
        paddingTop: 2,
        paddingBottom: 2
    },
    tags: {
        color: '#2FAFCC',
        fontSize: 13
    },
    date: {
        fontSize: 12
    }
});

interface EmployeesListState {
    imgContainerSize: number;
}

export class EmployeesListItem extends React.Component<EmployeesListItemProps, EmployeesListState> {
    public state: EmployeesListState = {
        imgContainerWidth: 0
    } as any;

    public onLayout = (e: LayoutEvent) => {
        this.setState({
            imgContainerSize: Math.min(e.nativeEvent.layout.width, e.nativeEvent.layout.height)
        });
    }

    public render() {
        const imgStyle = StyleSheet.flatten([{
            width: this.state.imgContainerSize,
            height: this.state.imgContainerSize
        }]);

        const employeeName = this.props.employee ? this.props.employee.name : 'Unknown';
        const photo = this.props.employee ? this.props.employee.photo : null;
        const mimeType = photo ? photo.mimeType : null;
        const base64 = photo ? photo.base64 : null;

        return (
            <TouchableHighlight>
                <View style={styles.layout}>
                    <View style={styles.imgContainer} onLayout = {this.onLayout}>
                        <Avatar mimeType={mimeType} photoBase64={base64} style={imgStyle}/>
                    </View>
                    <View style={styles.info}>
                        <Text style={styles.title}>{employeeName}</Text>
                    </View>
                </View>
            </TouchableHighlight>
        );
    }
}