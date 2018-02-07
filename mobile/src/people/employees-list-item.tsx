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
        height: 36,
        flex: 1,
        flexDirection: 'row',
        justifyContent: 'center',
    },
    imgContainer: {
        marginTop: 5,
        marginLeft: 28,
        flex: 1
    },
    img: {
        flex: 1
    },
    info: {
        flex: 6,
        marginRight: 28,
        justifyContent: 'center'
    },
    baseText: {
        fontFamily: 'Helvetica-Light',
        fontSize: 12,
        textAlign: 'left'
    },
    name: {
        fontWeight: 'bold'
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
            width: 25,
            height: 25
        }]);

        const employeeName = this.props.employee ? this.props.employee.name.replace(',', '') : 'Unknown';
        const employeePosition = this.props.employee ? this.props.employee.position : 'Unknown';
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
                        <Text style={styles.baseText}><Text style={styles.name}>{employeeName}</Text><Text>, {employeePosition}</Text></Text>
                    </View>
                </View>
            </TouchableHighlight>
        );
    }
}