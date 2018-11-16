import React from 'react';
import { TouchableHighlight, StyleSheet, Platform, Text, View, Image, LayoutChangeEvent, TouchableOpacity } from 'react-native';
import { Avatar } from '../people/avatar';
import { Feed } from '../reducers/feeds/feed.model';
import { feedStyles as styles } from './styles';
import { Employee } from '../reducers/organization/employee.model';
import { StyledText } from '../override/styled-text';

interface FeedMessageProps {
    message: Feed;
    employee: Employee;
    onAvatarClicked: (e: Employee) => void;
}

export class FeedMessage extends React.Component<FeedMessageProps> {
    public render() {
        const message = this.props.message;
        const employee = this.props.employee;
        const photo = employee ? employee.photoUrl : null;
        const formattedDate = message.datePosted.format('MMMM D, YYYY');

        const isDisabledClick = !this.props.employee;

        const avatarContent = isDisabledClick ?
            <Avatar photoUrl={photo} /> :
            <TouchableOpacity onPress={this.onAvatarClicked} style={styles.touchableOpacityContainer} >
                <Avatar photoUrl={photo} />
            </TouchableOpacity> ;

        return (
            <View>
                <View style={styles.layout}>
                    <View style={styles.imgContainer}>
                        {avatarContent}
                    </View>
                    <View style={styles.info}>
                        <StyledText style={styles.title}>{this.props.message.title}</StyledText>
                        <StyledText>
                            <Text style={styles.text}>{this.props.message.text}</Text>
                        </StyledText>
                        <StyledText style={styles.date}>{formattedDate}</StyledText>
                    </View>
                </View>
            </View>
        );
    }

    private onAvatarClicked = () => {
        this.props.onAvatarClicked(this.props.employee);
    }
}
